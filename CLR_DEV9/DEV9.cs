﻿using PSE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace CLRDEV9
{
    class CLR_DEV9
    {
        private static string LogFolderPath = "logs";
        private static string IniFolderPath = "inis";
        private static DEV9.DEV9_State dev9 = null;
        const bool doLog = true;
        private static void LogInit()
        {
            if (doLog)
            {
                //some legwork to setup the logger
                Dictionary<ushort, string> logsources = new Dictionary<ushort, string>();
                IEnumerable<DEV9LogSources> sources = Enum.GetValues(typeof(DEV9LogSources)).Cast<DEV9LogSources>();

                foreach (DEV9LogSources source in sources)
                {
                    logsources.Add((ushort)source, source.ToString());
                }

                if (LogFolderPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    CLR_PSE_PluginLog.Open(LogFolderPath.TrimEnd('/'), "CLR_DEV9.log", logsources);
                }
                else
                {
                    CLR_PSE_PluginLog.Open(LogFolderPath, "CLR_DEV9.log", logsources);
                }
                //TODO Set Log Options
#if DEBUG
                //Info is defualt
                CLR_PSE_PluginLog.SetFileLevel(SourceLevels.All);
                CLR_PSE_PluginLog.SetSourceLogLevel(SourceLevels.All, (int)DEV9LogSources.ATA);
#endif
                CLR_PSE_PluginLog.SetSourceUseStdOut(true, (int)DEV9LogSources.Dev9);
                CLR_PSE_PluginLog.SetSourceUseStdOut(true, (int)DEV9LogSources.SMAP);
                CLR_PSE_PluginLog.SetSourceUseStdOut(true, (int)DEV9LogSources.Winsock);
                CLR_PSE_PluginLog.SetSourceUseStdOut(true, (int)DEV9LogSources.NetAdapter);
            }
        }

        public static Int32 Init()
        {
            try
            {
                LogInit();
                Log_Info("Init");
                dev9 = new DEV9.DEV9_State();
                Log_Info("Init ok");
                return 0;
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                return -1;
            }
        }
        public static Int32 Open(IntPtr winHandle)
        {
            try
            {
                Log_Info("Open");
                Config.LoadConf(IniFolderPath, "CLR_DEV9.ini");

                if (DEV9Header.config.Hdd.Contains("\\") || DEV9Header.config.Hdd.Contains("/"))
                    return dev9.Open(DEV9Header.config.Hdd);
                else
                    return dev9.Open(IniFolderPath + "\\" + DEV9Header.config.Hdd);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                return -1;
            }
        }
        public static void Close()
        {
            try
            {
                dev9.Close();
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static void Shutdown()
        {
            try
            {
                Log_Info("Shutdown");
                CLR_PSE_PluginLog.Close();
                //PluginLog.Close(); //fclose(dev9Log);

                if (irqHandle.IsAllocated)
                {
                    irqHandle.Free(); //allow garbage collection
                }
                //Do dispose()? (of what?)
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static void SetSettingsDir(string dir)
        {
            try
            {
                IniFolderPath = dir;
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static void SetLogDir(string dir)
        {
            try
            {
                LogFolderPath = dir;
                //LogInit();
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        public static byte DEV9read8(uint addr)
        {
            try
            {
                return dev9.DEV9read8(addr);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static ushort DEV9read16(uint addr)
        {
            try
            {
                return dev9.DEV9_Read16(addr);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static uint DEV9read32(uint addr)
        {
            try
            {
                return dev9.DEV9_Read32(addr);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        public static void DEV9write8(uint addr, byte value)
        {
            try
            {
                dev9.DEV9_Write8(addr, value);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static void DEV9write16(uint addr, ushort value)
        {
            try
            {
                dev9.DEV9_Write16(addr, value);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static void DEV9write32(uint addr, uint value)
        {
            try
            {
                dev9.DEV9_Write32(addr, value);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        unsafe public static void DEV9readDMA8Mem(byte* memPointer, int size)
        {
            try
            {
                System.IO.UnmanagedMemoryStream pMem = new System.IO.UnmanagedMemoryStream(memPointer, size, size, System.IO.FileAccess.Write);
                dev9.DEV9_ReadDMA8Mem(pMem, size);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        unsafe public static void DEV9writeDMA8Mem(byte* memPointer, int size)
        {
            try
            {
                System.IO.UnmanagedMemoryStream pMem = new System.IO.UnmanagedMemoryStream(memPointer, size, size, System.IO.FileAccess.Read);
                dev9.DEV9_WriteDMA8Mem(pMem, size);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        public static void DEV9async(uint cycles)
        {
            try
            {
                dev9.DEV9_Async(cycles);
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        public static void DEV9irqCallback(PSE.CLR_PSE_Callbacks.CLR_CyclesCallback callback)
        {
            try
            {
                DEV9Header.DEV9irq = callback;
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        static GCHandle irqHandle;
        public static int _DEV9irqHandler()
        {
            try
            {
                return dev9._DEV9irqHandler();
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }
        public static PSE.CLR_PSE_Callbacks.CLR_IRQHandler DEV9irqHandler()
        {
            try
            {
                // Pass our handler to pcsx2.
                if (irqHandle.IsAllocated)
                {
                    irqHandle.Free(); //allow garbage collection
                }
                Log_Info("Get IRQ");
                PSE.CLR_PSE_Callbacks.CLR_IRQHandler fp = new PSE.CLR_PSE_Callbacks.CLR_IRQHandler(_DEV9irqHandler);
                irqHandle = GCHandle.Alloc(fp); //prevent GC
                return fp;
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        //freeze
        //config
        //about
        //test
        public static int Test()
        {
            try
            {
                return 0;
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                return 1;
            }
        }
        public static void Configure()
        {
            try
            {
                Config.LoadConf(IniFolderPath, "CLR_DEV9.ini");
                Config.DoConfig(IniFolderPath, "CLR_DEV9.ini");
                Config.SaveConf(IniFolderPath, "CLR_DEV9.ini");
            }
            catch (Exception e)
            {
                CLR_PSE_PluginLog.MsgBoxError(e);
                throw e;
            }
        }

        private static void Log_Error(string str)
        {
            CLR_PSE_PluginLog.WriteLine(TraceEventType.Error, (int)DEV9LogSources.PluginInterface, str);
        }
        private static void Log_Info(string str)
        {
            CLR_PSE_PluginLog.WriteLine(TraceEventType.Information, (int)DEV9LogSources.PluginInterface, str);
        }
        private static void Log_Verb(string str)
        {
            CLR_PSE_PluginLog.WriteLine(TraceEventType.Verbose, (int)DEV9LogSources.PluginInterface, str);
        }
    }
}
