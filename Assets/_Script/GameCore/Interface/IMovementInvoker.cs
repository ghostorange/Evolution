using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore {
    public interface IMovementInvoker : IInvoker {
        float MoveSpeed {
            get;
            set;
        }
        Transform Actor {
            get;
        }
        void InvokeMoveCMD(IMoveCMD cmd);
    }
}