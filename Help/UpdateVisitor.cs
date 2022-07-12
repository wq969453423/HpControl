using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 子端.Model;

namespace 子端.Help
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }


        public OutPutAllDto GetSystemInfo()
        {
            OutPutAllDto res = new OutPutAllDto();
            UpdateVisitor updateVisitor = new UpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.RAMEnabled = true;
            computer.HDDEnabled = true;
            computer.FanControllerEnabled = true;
            computer.Accept(updateVisitor);
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        SensorType type = computer.Hardware[i].Sensors[j].SensorType;
                        var item = computer.Hardware[i].Sensors[j];
                        if (type == SensorType.Power && item.Name == "CPU Package")
                        {
                            res.CpuPower = (decimal)item.Max;
                        }
                        else if (type == SensorType.Clock)
                        {
                            if ((decimal)item.Max > res.CpuFrequency)
                            {
                                res.CpuFrequency = (decimal)item.Max;
                            }
                        }
                        else if (type == SensorType.Temperature && item.Name == "CPU Package")
                        {
                            res.CpuTemperature = (decimal)item.Max;
                        }
                        else if (type == SensorType.Load && item.Name == "CPU Total")
                        {
                            res.CpuTemperature = (decimal)item.Max;
                        }
                    }
                }
                else if (computer.Hardware[i].HardwareType == HardwareType.RAM)
                {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        SensorType type = computer.Hardware[i].Sensors[j].SensorType;
                        var item = computer.Hardware[i].Sensors[j];
                        if (type == SensorType.Load)
                        {
                            res.MemoryLoad = (decimal)item.Max;
                        }
                        else if (type == SensorType.Data && item.Name == "Used Memory")
                        {
                            res.MemorySize = (decimal)item.Max;
                        }
                    }
                }
                else if (computer.Hardware[i].HardwareType == HardwareType.HDD) {
                    for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
                    {
                        SensorType type = computer.Hardware[i].Sensors[j].SensorType;
                        var item = computer.Hardware[i].Sensors[j];
                        if (type == SensorType.Temperature)
                        {
                            if ((decimal)item.Max> res.SsdTemperature)
                            {
                                res.SsdTemperature = (decimal)item.Max;
                            }
                            
                        }
                        else if (type == SensorType.Load)
                        {
                            if ((decimal)item.Max > res.SsdLoad)
                            {
                                res.SsdLoad = (decimal)item.Max;
                            }
                        }
                    }
                }
            }
            computer.Close();
            return res;
        }

    }
}
