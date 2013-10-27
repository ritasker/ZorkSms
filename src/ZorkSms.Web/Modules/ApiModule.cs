using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using ZorkSms.Data;
using ZorkSms.Data.Models;
using System;
using ZorkSms.Core;
using Clockwork;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace ZorkSms.Web.Modules
{
    public class ApiModule : NancyModule
    {
        private static object sessionLock = new object();
        private readonly SessionRepository _sessionRepository;

        public ApiModule(SessionRepository sessionRepository) : base("/api")
        {
            _sessionRepository = sessionRepository;

            Post["/ReceiveSms"] = o => ReceiveSms();

            Get["/TestReceive"] = o => { return this.View["TestReceive"]; };
            Post["/TestReceive"] = o => { return TestReceive().Result; };
        }

        private async Task<dynamic> ReceiveSms()
        {
            var smsMessage = this.Bind<SmsMessage>();

            var response = await HandleSms(smsMessage);

            var clockworkApi = new API("508ed11cab000a796881e015fc5e022daf1bb6d3");
            if (!string.IsNullOrWhiteSpace(response))
            {
                clockworkApi.Send(new SMS { To = smsMessage.From, Message = response });
            }
            else
            {
                clockworkApi.Send(new SMS { To = smsMessage.From, Message = "Thanks for playing. You will stop receiving messages." });
            }
                        
            return HttpStatusCode.OK;
        }

        private async Task<string> HandleSms(SmsMessage smsMessage)
        {
            bool isStop = string.Equals(smsMessage.Content, "STOP", StringComparison.OrdinalIgnoreCase);

            if (isStop) {
                _sessionRepository.Delete(x => x.PhoneNumber == smsMessage.From);
                return string.Empty;
            }

            bool isNewGame = string.Equals(smsMessage.Content, "NEW GAME", StringComparison.OrdinalIgnoreCase);

            SessionModel session = _sessionRepository.FindByPhoneNumber(smsMessage.From);

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

            if (session != null)
            {
                session.Commands = commands;
                _sessionRepository.Update(session);
            }
            else
            {
                session = new SessionModel { PhoneNumber = smsMessage.From, Commands = commands };
                _sessionRepository.Add(session);
            }

            return messages.Last();
        }

        private async Task<string> TestReceive()
        {
            var smsMessage = this.Bind<SmsMessage>();

            var response = await HandleSms(smsMessage);

            return response;
        }
    }
}