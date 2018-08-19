using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore{
    /// <summary>
    /// 移动命令
    /// </summary>
    public interface IMoveCMD : ICommand {
        Vector3 dircion {
            get;
        }
    }
}