using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Validation.Rules;
using ZorkSms.Data;
using ZorkSms.Data.Models;
using System;
using ZorkSms.Core;
using System.IO;
using Clockwork;
using System.Resources;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace ZorkSms.Web.Modules
{
    public class ApiModule : NancyModule
    {
        private static object sessionLock = new object();
        private readonly SmsRepository _smsRepository;
        private readonly SessionRepository _sessionRepository;

        public ApiModule(SmsRepository smsRepository, SessionRepository sessionRepository) : base("/api")
        {
            _smsRepository = smsRepository;
            _sessionRepository = sessionRepository;

            Get["/ping"] = o => HttpStatusCode.OK;

            Post["/ReceiveSms"] = o => ReceiveSms();

            Get["/Messages"] = o => Messages();

            Get["/TestReceive"] = o => { return this.View["TestReceive"]; };
            Post["/TestReceive"] = o => { return TestReceive().Result; };
        }

        private dynamic Messages()
        {
            IList<SmsMessage> smsMessages = _smsRepository.Collection.FindAll().ToList();
            return new JsonResponse(smsMessages, new DefaultJsonSerializer());
        }

        private async Task<dynamic> ReceiveSms()
        {
            var smsMessage = this.Bind<SmsMessage>();

            var response = await HandleSms(smsMessage);

            var clockworkApi = new API("508ed11cab000a796881e015fc5e022daf1bb6d3");
            clockworkApi.Send(new SMS { To = smsMessage.From, Message = response });
                        
            _smsRepository.Add(smsMessage);
            
            return HttpStatusCode.OK;
        }

        private async Task<string> TestReceive()
        {
            var smsMessage = this.Bind<SmsMessage>();

            var response = await HandleSms(smsMessage);

            _smsRepository.Add(smsMessage);

            return response;
        }

        private async Task<string> HandleSms(SmsMessage smsMessage)
        {
            SessionModel session = _sessionRepository.FindByPhoneNumber(smsMessage.From);

            bool isNewGame = string.Equals(smsMessage.Content, "NEW GAME", StringComparison.OrdinalIgnoreCase);

            Game game = null;
            var assembly = Assembly.GetExecutingAssembly();
            var resource = assembly.GetManifestResourceStream("ZorkSms.Web.minizork.z3");

            byte[] storyBytes = new byte[resource.Length];
            resource.Read(storyBytes, 0, (int)resource.Length);
            resource.Close();

            List<string> commands = new List<string>();

            if (!isNewGame && session != null)
            {
                commands.AddRange(session.Commands);
                commands.Add(smsMessage.Content);
            }

            game = Game.CreateNew(storyBytes, commands);

            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            List<string> messages = new List<string>();
            game.PrintCompleted += (sender, args) =>
            {
                messages.Add(string.Join("\n", args.Lines));
                wait.Set();
            };

            game.Start();
            wait.WaitOne();

            var newSession = new SessionModel { PhoneNumber = smsMessage.From, Commands = commands };

            lock (sessionLock)
            {
                session = _sessionRepository.FindByPhoneNumber(smsMessage.From);

                if (session != null)
                {
                    _sessionRepository.Update(newSession);
                }
                else
                {
                    _sessionRepository.Add(newSession);
                }
            }

            return messages.Last();
        }
    }
}