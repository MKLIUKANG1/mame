﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zanac.MAmidiMEmo.Gui
{
    public partial class FormProgress : FormBase
    {
        /// <summary>
        /// Thread Safe
        /// </summary>
        public string Message
        {
            get
            {
                string rv = null;
                if (labelMessage.IsHandleCreated)
                {
                    labelMessage.Invoke(new MethodInvoker(() =>
                    {
                        if (!labelMessage.IsDisposed)
                            rv = labelMessage.Text;
                    }));
                }
                else
                {
                    rv = labelMessage.Text;
                }
                return rv;
            }
            set
            {
                if (labelMessage.IsHandleCreated)
                {
                    labelMessage.Invoke(new MethodInvoker(() =>
                    {
                        if (!labelMessage.IsDisposed)
                            labelMessage.Text = value;
                    }));
                }
                else
                {
                    labelMessage.Text = value;
                }
            }
        }

        /// <summary>
        /// Thread Safe
        /// </summary>
        public int Percentage
        {
            get
            {
                int rv = 0;
                if (metroProgressBar1.IsHandleCreated)
                {
                    metroProgressBar1.Invoke(new MethodInvoker(() =>
                    {
                        if (!labelMessage.IsDisposed)
                            rv = metroProgressBar1.Value;
                    }));
                }
                else
                {
                    rv = metroProgressBar1.Value;
                }
                return rv;
            }
            set
            {
                if (metroProgressBar1.IsHandleCreated)
                {
                    metroProgressBar1.Invoke(new MethodInvoker(() =>
                    {
                        if (!metroProgressBar1.IsDisposed)
                        {
                            if (value < 0)
                                metroProgressBar1.ProgressBarStyle = ProgressBarStyle.Marquee;
                            else
                            {
                                metroProgressBar1.ProgressBarStyle = ProgressBarStyle.Continuous;
                                metroProgressBar1.Value = value;
                            }
                        }
                    }));
                }
                else
                {
                    if (value < 0)
                        metroProgressBar1.ProgressBarStyle = ProgressBarStyle.Marquee;
                    else
                    {
                        metroProgressBar1.ProgressBarStyle = ProgressBarStyle.Continuous;
                        metroProgressBar1.Value = value;
                    }
                }
            }
        }

        public FormProgress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialMessage"></param>
        /// <param name="taskAction"></param>
        public static void RunDialog(string initialMessage, Action<FormProgress> taskAction) 
        {
            RunDialog(initialMessage, taskAction, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialMessage"></param>
        /// <param name="taskAction"></param>
        public static void RunDialog(string initialMessage, Action<FormProgress> taskAction, Action cancelHandler)
        {
            if (FormMain.AppliactionForm != null)
            {
                FormMain.AppliactionForm.Invoke(new MethodInvoker(() =>
                {
                    runDialogCore(FormMain.AppliactionForm, initialMessage, taskAction, cancelHandler);
                }));
            }
            else
            {
                runDialogCore(null, initialMessage, taskAction, cancelHandler);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="initialMessage"></param>
        /// <param name="action"></param>
        private static void runDialogCore(Form parent, string initialMessage, Action<FormProgress> action, Action cancelHandler)
        {
            using (FormProgress f = new FormProgress())
            {
                if (parent == null)
                    f.StartPosition = FormStartPosition.CenterScreen;

                if (cancelHandler != null)
                {
                    f.metroButtonCancel.Enabled = true;
                    f.metroButtonCancel.Click += (s, e) =>
                    {
                        cancelHandler.Invoke();
                    };
                }

                f.CreateControl();

                f.Message = initialMessage;

                f.Shown += (s, e) =>
                {
                    var t = Task.Run(new Action(() =>
                    {
                        var now = DateTime.Now;

                        action(f);

                        // Dummy wait for elegant UI
                        var span = DateTime.Now - now;
                        if (span.TotalMilliseconds < 500)
                            Thread.Sleep((int)(500 - span.TotalMilliseconds));

                        f.Invoke(new MethodInvoker(() => { f.Close(); }));
                    }));
                };
                f.ShowDialog(parent);
            }
        }

    }
}
