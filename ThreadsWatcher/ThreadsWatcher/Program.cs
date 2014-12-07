using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Diagnostics;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorMetadata;
using System.Windows.Forms;

namespace ThreadsWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Process name is not set!", "Attention!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                ThreadsAnalyzer(args[0]);
            }
        }



        private static void ThreadsAnalyzer(string processName)
        {
            int processId = 0;
            try
            {
                processId = Process.GetProcessesByName(processName)[0].Id;
                MDbgEngine debugger = new MDbgEngine();
                MDbgProcess process = null;
                process = debugger.Attach(processId);
                DrainAttach(debugger, process);
                CreateHTMLTable(processName);

                foreach (MDbgThread thread in process.Threads)
                {
                    string threadName = "";
                    int threadId = 0;
                    CorReferenceValue refVal = thread.CorThread.ThreadVariable.CastToReferenceValue();

                    if (refVal.IsNull)
                        continue;

                    CorValue corVal = refVal.Dereference();
                    object obj = corVal.CastToObjectValue();
                    CorObjectValue val = obj as CorObjectValue;

                    if (val != null)
                    {
                        CorClass corClass = val.Class;
                        MDbgModule classModule = process.Modules.Lookup(corClass.Module);
                        Type classType = classModule.Importer.GetType(corClass.Token);
                        System.Reflection.FieldInfo[] fileds = classType.GetFields();
                        System.Reflection.FieldInfo fi;

                        for (int i = 0; i < fileds.Length; i++)
                        {
                            fi = fileds[i];
                            CorValue fieldValue = null;
                            MDbgValue mdbgVal = null;

                            try
                            {
                                switch (fi.Name)
                                {
                                    case "m_Name":
                                        fieldValue = val.GetFieldValue(corClass, fi.MetadataToken);
                                        mdbgVal = new MDbgValue(process, fi.Name, fieldValue);
                                        threadName = mdbgVal.CorValue.CastToReferenceValue().Dereference().CastToStringValue().String;
                                        break;
                                    case "m_ManagedThreadId":
                                        fieldValue = val.GetFieldValue(corClass, fi.MetadataToken);
                                        mdbgVal = new MDbgValue(process, fi.Name, fieldValue);
                                        threadId = (int)mdbgVal.CorValue.CastToGenericValue().GetValue();
                                        break;
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }

                    
                    string[] frames = StackInfo(thread.Frames);

                    if (threadName == "")
                        threadName = "No thread name";

                    ConsoleOutput(threadId.ToString(), threadName, frames);

                }

                process.CorProcess.Detach();
            }

            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show(String.Format("Process {0} is already being debugged!", processName), 
                    "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            { }
            
            Console.WriteLine("</TABLE> ");
        }


        //Return an information about StackTrace
        private static string[] StackInfo(IEnumerable<MDbgFrame> threadFrames)
        {
            string[] frames = new string[threadFrames.ToArray().Length];
            int k = 0;
            foreach (MDbgFrame f in threadFrames)
            {
                string frameString = GetFrameString(f, false) != null ? GetFrameString(f, false).Trim() : "";
                frames[k] = frameString;
                k++;
            }

            return frames;
        }


        //Output thread info
        private static void ConsoleOutput(string ID, string Name, string[] frames)
        {
            Console.WriteLine("<TR>");
            Console.WriteLine(String.Format(@"<TD style=""vertical-align:top"">{0}</TD>", ID));
            Console.WriteLine(String.Format(@"<TD style=""vertical-align:top"">{0}</TD>", Name));
            Console.WriteLine("<TD>");
           
            if (frames.Length > 0)
            {
                for (int i = 0; i < frames.Length-1; i++)
                {
                    Console.WriteLine(frames[i]+"<br>");
                }
            } 
            
            Console.WriteLine("</TD></TR>");
        }


        //Create table using for structured output
        private static void CreateHTMLTable(string processName)
        {
            string[] columnsNames = new string[] { "ManagedThreadID", "ThreadName", "CallStack" };

            string tableTitle = String.Format(@"<TABLE  BORDER=""2"">
               <TR>
                  <TH COLSPAN=""3"">
                     <H3><BR>Table of {0} Process Managed Threads</H3>
                  </TH>
               </TR>
                  <TH>{1}</TH>
                  <TH>{2}</TH>
	              <TH>{3}</TH>", 
                   processName,columnsNames[0],columnsNames[1],columnsNames[2] );

            Console.WriteLine(tableTitle);
        }


 
        private static string GetFrameString(MDbgFrame frame, bool withArgs = false)
        {
            try
            {
                if (withArgs)
                {
                    if (frame.IsManaged && !frame.Function.FullName.StartsWith("System."))
                        return string.Format("{0}({1})", frame.Function.FullName, string.Join(", ", frame.Function.GetArguments(frame).Select(arg => string.Format("{0} = {1}", arg.Name, arg.GetStringValue(false)))));
                }
                else
                {
                    if (frame.IsInfoOnly)
                        return null;

                    string retVal = frame.Function.FullName + " (";
                    foreach (MDbgValue value2 in frame.Function.GetArguments(frame))
                    {
                        retVal += value2.TypeName + ", ";
                    }
                    retVal = retVal.TrimEnd(", ".ToCharArray()) + ")";
                    return retVal;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        //Complete attachment
        private static void DrainAttach(MDbgEngine debugger, MDbgProcess proc)
        {
            bool fOldStatus = debugger.Options.StopOnNewThread;
            debugger.Options.StopOnNewThread = false; 

            proc.Go().WaitOne();
            Debug.Assert(proc.StopReason is AttachCompleteStopReason);
            debugger.Options.StopOnNewThread = true; 

            while (proc.CorProcess.HasQueuedCallbacks(null))
            {
                proc.Go().WaitOne();
                Debug.Assert(proc.StopReason is ThreadCreatedStopReason);
            }

            debugger.Options.StopOnNewThread = fOldStatus;
        }

    }
}
