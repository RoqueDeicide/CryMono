﻿using CryEngine;
using CryEngine.FlowSystem;

namespace FGPlugin
{
    class SampleNode : IFlowNode
    {
        public SPluginRegister RegisterWithPluginSystem()
        {
            SPluginRegister pluginRegister = new SPluginRegister(true);

            pluginRegister.nodesFirst = CG2AutoRegFlowNodeBase.m_pFirst;
            pluginRegister.nodesLast = CG2AutoRegFlowNodeBase.m_pLast;

            return pluginRegister;
        }

        // Input and Output port enumerations must be placed in the order they are declared in GetConfiguration()
        enum EInputPorts
        {
            EIP_Start,
            EIP_Cancel
        };

        enum EOutputPorts
        {
            EOP_Started,
            EOP_Cancelled
        };

        public SampleNode()
        {
        }

        ~SampleNode()
        {
        }

        ////////////////////////////////////////////////////
        public SFlowNodeConfig GetConfiguration()
	    {
            CryConsole.LogAlways("Retrieving flownode configuration");

            SFlowNodeConfig config = new SFlowNodeConfig();

            SInputPortConfig res = new SInputPortConfig("Start", EInputPortType.Void, "Start input");
		    SInputPortConfig[] inputs =
		    {
                new SInputPortConfig("Start", EInputPortType.Void, "Start input"),
                new SInputPortConfig("Cancel", EInputPortType.Void, "Cancel input")
		    };

		    SOutputPortConfig[] outputs =
		    {
			    new SOutputPortConfig("Started", "Called on start"),
                new SOutputPortConfig("Cancelled", "Called on cancel")
		    };

		    config.pInputPorts = inputs;
		    config.pOutputPorts = outputs;
		    config.sDescription = "Does nothing!";
            config.category = EFlowNodeCategory.EFLN_DEBUG;

            return config;
	    }

        public void ProcessEvent(IFlowNode.EFlowEvent _event, IFlowNode.SActivationInfo nodeInfo)
        {
            switch (_event)
            {
                case EFlowEvent.Activate:
                    {/*
                        if (IsPortActive(nodeInfo, (int)EInputPorts.EIP_Start))
                        {
                            ActivateOutput(nodeInfo, (int)EOutputPorts.EOP_Started);
                        }*/
                    }
                    break;
            }
        }
    }
}