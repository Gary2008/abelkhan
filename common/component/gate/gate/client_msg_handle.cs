﻿using System;
using System.Collections;

namespace gate
{
	public class client_msg_handle
	{
		public client_msg_handle(clientmanager _clientmanager_, logicmanager _logicmanager_, hubmanager _hubmanager_, service.timerservice _timerservice_)
		{
			_clientmanager = _clientmanager_;
			_logicmanager = _logicmanager_;
            _hubmanager = _hubmanager_;
            _timerservice = _timerservice_;
		}

		public void connect_server(string uuid, long tick)
		{
			if (!_clientmanager.has_client(uuid)) {
				System.Console.WriteLine("client " + uuid + " connected");

				var _proxy = _clientmanager.reg_client(uuid, juggle.Imodule.current_ch, _timerservice.Tick, tick);
                _proxy.connect_gate_sucessa();
            }
		}

		public void cancle_server()
		{
			_clientmanager.unreg_client(juggle.Imodule.current_ch);
        }

        public void connect_logic(string client_uuid, string logic_uuid)
        {
            logicproxy _logicproxy = _logicmanager.get_logic(logic_uuid);
            if (_logicproxy != null)
            {
                _clientmanager.reg_client_logic(client_uuid, _logicproxy);
                _logicproxy.client_connect(client_uuid);
            }
        }

        public void disconnect_logic(string client_uuid, string logic_uuid)
        {
            logicproxy _logicproxy = _logicmanager.get_logic(logic_uuid);
            if (_logicproxy != null)
            {
                _logicproxy.client_disconnect(client_uuid);
                _clientmanager.unreg_client_logic(juggle.Imodule.current_ch);
            }
        }

        public void connect_hub(string client_uuid, string hub_name)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);
            if (_hubproxy != null)
            {
                _clientmanager.reg_client_hub(client_uuid, _hubproxy);
                _hubproxy.client_connect(client_uuid);
            }
        }

        public void disconnect_hub(string client_uuid, string hub_name)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);
            if (_hubproxy != null)
            {
                _hubproxy.client_disconnect(client_uuid);
                _clientmanager.unreg_client_hub(juggle.Imodule.current_ch);
            }
        }

        public void forward_client_call_logic(string logic_uuid, string module, string func, ArrayList argv)
		{
            logicproxy _logicproxy = _logicmanager.get_logic(logic_uuid);

            if (_logicproxy != null)
			{
				string uuid = _clientmanager.get_client_uuid(_clientmanager.get_clientproxy(juggle.Imodule.current_ch));
				_logicproxy.client_call_logic (uuid, module, func, argv);
			}
		}

        public void forward_client_call_hub(string hub_name, string module, string func, ArrayList argv)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);

            if (_hubproxy != null)
            {
                string uuid = _clientmanager.get_client_uuid(_clientmanager.get_clientproxy(juggle.Imodule.current_ch));
                _hubproxy.client_call_hub(uuid, module, func, argv);
            }
        }

        public void heartbeats(long clockt)
		{
			_clientmanager.refresh_and_check_client(juggle.Imodule.current_ch, _timerservice.Tick, clockt);
		}

		private clientmanager _clientmanager;
		private logicmanager _logicmanager;
        private hubmanager _hubmanager;
		private service.timerservice _timerservice;
	}
}

