﻿using Murong_Xue.Logging;
using Murong_Xue.Logging.OutputHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steward_Zhou
{
    internal class LogOutputListView : IOutputModule
    {
        ListBox lb;
        public LogOutputListView(ListBox ListBox)
        {
            lb = ListBox;
        }
        public void WriteMsg(LogMsg msg, bool InteractiveMode)
        {
        //TODO implement a try catch that just dumps it incase a log is being put through
        //when the app is closing (happened) Or somehow check if the winform is closed/being closed
            if (lb.InvokeRequired)
                lb.Invoke((MethodInvoker)delegate
                {
                    lb.Items.Add(msg.ToString());
                });
            else lb.Items.Add(msg.ToString()); //doubt this ever happenss
        }
        public void Dispose() { }
    }
}
