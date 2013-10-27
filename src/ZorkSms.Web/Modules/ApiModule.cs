using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
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
        private readonly SmsRepository _smsRepository;
        private readonly SessionRepository _sessionRepository;

        public ApiModule(SmsRepository smsRepository, SessionRepository sessionRepository) : base("/api")
        {
            _smsRepository = smsRepository;
            _sessionRepository = sessionRepository;

            Get["/ping"] = o => HttpStatusCode.OK;

            Post["/ReceiveSms"] = o => ReceiveSms();

            Get["/Messages"] = o => Messages();
        }

        private dynamic Messages()
        {
            IList<SmsMessage> smsMessages = _smsRepository.Collection.FindAll().ToList();
            return new JsonResponse(smsMessages, new DefaultJsonSerializer());
        }

        private dynamic ReceiveSms()
        {
            var smsMessage = this.Bind<SmsMessage>();

            var thread = new Thread(new ParameterizedThreadStart(HandleSms));
            thread.Start(smsMessage);
            
            _smsRepository.Add(smsMessage);
            
            return HttpStatusCode.OK;
        }

        private void HandleSms(object arg)
        {
            SmsMessage smsMessage = (SmsMessage)arg;
            bool isNewGame = string.Equals(smsMessage.Content, "NEW GAME", StringComparison.OrdinalIgnoreCase);

            SessionModel session = null;
            if (!isNewGame)
            {
                session = _sessionRepository.FindByPhoneNumber(smsMessage.From);
            }

            Game game;
            if (session != null)
            {
                game = Game.Restore(session.Data);
            }
            else
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resource = assembly.GetManifestResourceStream("ZorkSms.Web.minizork.z3");

                byte[] storyBytes = new byte[resource.Length];
                resource.Read(storyBytes, 0, (int)resource.Length);
                resource.Close();

                game = Game.CreateNew(storyBytes);
                isNewGame = true;
            }

            game.PrintCompleted += (sender, args) =>
            {
                var message = string.Concat(args.Lines);

                var clockworkApi = new API("508ed11cab000a796881e015fc5e022daf1bb6d3");
                clockworkApi.Send(new SMS { To = smsMessage.From, Message = message });

                byte[] saveData = game.Save();
                var newSession = new SessionModel { PhoneNumber = smsMessage.From, Data = saveData };
                if (!isNewGame)
                {
                    _sessionRepository.Add(newSession);
                }
                else
                {
                    _sessionRepository.Update(newSession);
                }
            };

            if (!isNewGame)
            {
                game.Process(smsMessage.Content);
                
            }

            game.Start();
        }
    }
}