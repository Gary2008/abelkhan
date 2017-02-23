/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_logic : juggle.Imodule 
    {
        public gate_call_logic()
        {
			module_name = "gate_call_logic";
        }

        public delegate void reg_logic_sucesshandle();
        public event reg_logic_sucesshandle onreg_logic_sucess;
        public void reg_logic_sucess(ArrayList _event)
        {
            if(onreg_logic_sucess != null)
            {
                onreg_logic_sucess();
            }
        }

        public delegate void client_get_logichandle(String argv0);
        public event client_get_logichandle onclient_get_logic;
        public void client_get_logic(ArrayList _event)
        {
            if(onclient_get_logic != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_get_logic( argv0);
            }
        }

        public delegate void client_reg_logichandle(String argv0);
        public event client_reg_logichandle onclient_reg_logic;
        public void client_reg_logic(ArrayList _event)
        {
            if(onclient_reg_logic != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_reg_logic( argv0);
            }
        }

        public delegate void client_unreg_logichandle(String argv0);
        public event client_unreg_logichandle onclient_unreg_logic;
        public void client_unreg_logic(ArrayList _event)
        {
            if(onclient_unreg_logic != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_unreg_logic( argv0);
            }
        }

        public delegate void client_disconnecthandle(String argv0);
        public event client_disconnecthandle onclient_disconnect;
        public void client_disconnect(ArrayList _event)
        {
            if(onclient_disconnect != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_disconnect( argv0);
            }
        }

        public delegate void client_exceptionhandle(String argv0);
        public event client_exceptionhandle onclient_exception;
        public void client_exception(ArrayList _event)
        {
            if(onclient_exception != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_exception( argv0);
            }
        }

        public delegate void client_call_logichandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event client_call_logichandle onclient_call_logic;
        public void client_call_logic(ArrayList _event)
        {
            if(onclient_call_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onclient_call_logic( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
