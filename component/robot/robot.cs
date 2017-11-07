﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace robot
{
    public class client_proxy
    {
        public client_proxy(juggle.Ichannel ch)
        {
            uuid = System.Guid.NewGuid().ToString();
            _client_call_gate = new caller.client_call_gate(ch);
        }

        public void bind_udpchannel(juggle.Ichannel udp_ch)
        {
            _client_call_gate_fast = new caller.client_call_gate_fast(udp_ch);
        }

        public void heartbeats(Int64 tick)
        {
            _client_call_gate.heartbeats(tick);

            robot.timer.addticktime(5 * 1000, heartbeats);
        }

        public void refresh_udp_link(Int64 tick)
        {
            _client_call_gate_fast.refresh_udp_end_point();

            robot.timer.addticktime(10 * 1000, refresh_udp_link);
        }

        public void confirm_create_udp_link()
        {
            _client_call_gate_fast.confirm_create_udp_link(uuid);
        }

        public void connect_server(Int64 tick)
        {
            _client_call_gate.connect_server(uuid, tick);
        }

        public void cancle_server()
        {
            _client_call_gate.cancle_server();
        }

        public void connect_hub(string hub_name)
        {
            _client_call_gate.connect_hub(uuid, hub_name);
        }

        public void disconnect_hub(string hub_name)
        {
            _client_call_gate.disconnect_hub(uuid, hub_name);
        }

        public void call_hub(String hub_name, String module_name, String func_name, params object[] _argvs)
        {
            ArrayList _argvs_list = new ArrayList();
            foreach (var o in _argvs)
            {
                _argvs_list.Add(o);
            }

            _client_call_gate.forward_client_call_hub(hub_name, module_name, func_name, _argvs_list);
        }

        public String uuid;
        public caller.client_call_gate _client_call_gate;
        public caller.client_call_gate_fast _client_call_gate_fast;
    }

    public class robot
    {
        public robot(String[] args)
        {
            config.config _config = new config.config(args[0]);
            if (args.Length > 1)
            {
                _config = _config.get_value_dict(args[1]);
            }

            var log_level = _config.get_value_string("log_level");
            if (log_level == "debug")
            {
                log.log.logMode = log.log.enLogMode.Debug;
            }
            else if (log_level == "release")
            {
                log.log.logMode = log.log.enLogMode.Release;
            }
            var log_file = _config.get_value_string("log_file");
            log.log.logFile = log_file;
            var log_dir = _config.get_value_string("log_dir");
            log.log.logPath = log_dir;
            {
                if (!System.IO.Directory.Exists(log_dir))
                {
                    System.IO.Directory.CreateDirectory(log_dir);
                }
            }

            Int64 robot_num = _config.get_value_int("robot_num");
            
            _ip = _config.get_value_string("ip");
            _port = (short)_config.get_value_int("port");

            _udp_ip = _config.get_value_string("udp_ip");
            _udp_port = (short)_config.get_value_int("udp_port");

            timer = new service.timerservice();
            modulemanager = new common.modulemanager();

            _tcp_process = new juggle.process();
            _gate_call_client = new module.gate_call_client();
            _gate_call_client.onconnect_gate_sucess += on_ack_connect_gate;
            _gate_call_client.onconnect_hub_sucess += on_ack_connect_hub;
            _gate_call_client.oncall_client += on_call_client;
            _gate_call_client.onack_heartbeats += on_ack_heartbeats;
            _tcp_process.reg_module(_gate_call_client);
            _conn = new service.connectnetworkservice(_tcp_process);

            _udp_process = new juggle.process();
            _gate_call_client_fast = new module.gate_call_client_fast();
            _gate_call_client_fast.onconfirm_refresh_udp_end_point += on_confirm_refresh_udp_end_point;
            _gate_call_client_fast.oncall_client += on_call_client;
            _udp_process.reg_module(_gate_call_client_fast);
            _udp_conn = new service.udpconnectnetworkservice(_udp_process);

            _juggleservice = new service.juggleservice();
            _juggleservice.add_process(_tcp_process);

            proxys = new Dictionary<juggle.Ichannel, client_proxy>();

            _max_robot_num = robot_num;
            _robot_num = 0;
        }

        private void on_confirm_refresh_udp_end_point()
        {
            var _pre_proxy = proxys[juggle.Imodule.current_ch];
            _pre_proxy.confirm_create_udp_link();
        }

        public delegate void onConnectGateHandle();
        public event onConnectGateHandle onConnectGate;
        private void on_ack_connect_gate()
        {
            var _pre_proxy = proxys[juggle.Imodule.current_ch];
            var udp_ch = _udp_conn.connect(_udp_ip, _udp_port);
            _pre_proxy.bind_udpchannel(udp_ch);
            timer.addticktime(5 * 1000, _pre_proxy.heartbeats);
            timer.addticktime(10 * 1000, _pre_proxy.refresh_udp_link);

            if ( (++_robot_num) < _max_robot_num )
            {
                var ch = _conn.connect(_ip, _port);
                var proxy = new client_proxy(ch);
                proxys.Add(ch, proxy);
                proxy.connect_server(service.timerservice.Tick);
            }
            else
            {
                log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "all robots connected");
            }

            if (onConnectGate != null)
            {
                onConnectGate();
            }
        }

        public delegate void onConnectHubHandle(string _hub_name);
        public event onConnectHubHandle onConnectHub;
        private void on_ack_connect_hub(string _hub_name)
        {
            if (onConnectHub != null)
            {
                onConnectHub(_hub_name);
            }
        }

        private void on_ack_heartbeats()
        {
        }

        private void on_call_client(String module_name, String func_name, ArrayList argvs)
        {
            modulemanager.process_module_mothed(module_name, func_name, argvs);
        }

        public bool connect_server(Int64 tick)
        {
            try
            {
                var ch = _conn.connect(_ip, _port);
                var proxy = new client_proxy(ch);
                proxys.Add(ch, proxy);
                proxy.connect_server(tick);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        
        public Int64 poll()
        {
            Int64 tick_begin = timer.poll();
            _juggleservice.poll(tick_begin);

            System.GC.Collect();

            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

        public client_proxy get_client_proxy(juggle.Ichannel ch)
        {
            if (proxys.ContainsKey(ch))
            {
                return proxys[ch];
            }

            return null;
        }

        public static service.timerservice timer;
        public common.modulemanager modulemanager;

        private service.connectnetworkservice _conn;
        private juggle.process _tcp_process;
        private module.gate_call_client _gate_call_client;

        private service.udpconnectnetworkservice _udp_conn;
        private juggle.process _udp_process;
        private module.gate_call_client_fast _gate_call_client_fast;

        private service.juggleservice _juggleservice;

        private Dictionary<juggle.Ichannel, client_proxy> proxys;

        private string _ip;
        private short _port;
        private string _udp_ip;
        private short _udp_port;
        private Int64 _robot_num;
        private Int64 _max_robot_num;
    }
}
