﻿using System;
using System.Collections;

namespace gate
{
	public class clientproxy
	{
		public clientproxy(juggle.Ichannel ch)
		{
            client_ch = ch;
            _caller = new caller.gate_call_client(client_ch);
		}

        public void connect_gate_sucessa()
        {
            _caller.connect_gate_sucess();
        }

        public void connect_hub_sucess(string hub_name)
        {
            _caller.connect_hub_sucess(hub_name);
        }

		public void call_client(string module, string func, ArrayList argv)
		{
			_caller.call_client(module, func, argv);
		}

		private caller.gate_call_client _caller;
        public juggle.Ichannel client_ch;

    }
}

