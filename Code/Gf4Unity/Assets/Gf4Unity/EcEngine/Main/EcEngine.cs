﻿using System;
using System.Collections.Generic;
using System.Text;
using Eb;

namespace Ec
{
    public interface IEcEngineListener
    {
        //---------------------------------------------------------------------
        void init(EntityMgr entity_mgr, Entity et_node);

        //---------------------------------------------------------------------
        void release();
    }

    public struct EcEngineSettings
    {
        public string ProjectName;
        public string RootEntityType;
        public bool EnableCoSuperSocket;
        public bool EnableCoUCenter;
    }

    public sealed class EcEngine
    {
        //---------------------------------------------------------------------
        static EcEngine mInstance;
        IEcEngineListener mListener;
        EntityMgr mEntityMgr;

        //---------------------------------------------------------------------
        public static EcEngine Instance { get { return mInstance; } }
        public EntityMgr EntityMgr { get { return mEntityMgr; } }
        public EcEngineSettings Settings { get; private set; }
        public Entity EtNode { get; private set; }
        public ClientSuperSocket<DefSuperSocket> CoSuperSocket { get; private set; }
        public ClientUCenterSDK<DefUCenterSDK> CoUCenterSDK { get; private set; }

        //---------------------------------------------------------------------
        public EcEngine(ref EcEngineSettings settings, IEcEngineListener listener)
        {
            mInstance = this;
            mListener = listener;
            Settings = settings;

            mEntityMgr = new EntityMgr(1, "Client");

            mEntityMgr.regComponent<ClientAutoPatcher<DefAutoPatcher>>();
            mEntityMgr.regComponent<ClientNode<DefNode>>();
            mEntityMgr.regComponent<ClientSuperSocket<DefSuperSocket>>();
            mEntityMgr.regComponent<ClientUCenterSDK<DefUCenterSDK>>();

            mEntityMgr.regEntityDef<EtAutoPatcher>();
            mEntityMgr.regEntityDef<EtNode>();
            mEntityMgr.regEntityDef<EtSuperSocket>();
            mEntityMgr.regEntityDef<EtUCenterSDK>();

            EtNode = mEntityMgr.createEntity<EtNode>(null, null);
            var co_node = EtNode.getComponent<ClientNode<DefNode>>();
            CoSuperSocket = co_node.CoSuperSocket;
            CoUCenterSDK = co_node.CoUCenterSDK;

            // 通知业务层初始化
            if (mListener != null)
            {
                mListener.init(mEntityMgr, EtNode);
            }
        }

        //---------------------------------------------------------------------
        public void update(float elapsed_tm)
        {
            try
            {
                mEntityMgr.update(elapsed_tm);
            }
            catch (Exception ec)
            {
                EbLog.Error("EcEngine.update() Exception: " + ec);
            }
        }

        //---------------------------------------------------------------------
        public void close()
        {
            // 通知业务层销毁
            if (mListener != null)
            {
                mListener.release();
            }

            if (mEntityMgr != null)
            {
                mEntityMgr.destroy();
                mEntityMgr = null;
            }
        }
    }
}
