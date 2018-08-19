using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore{
    public interface ISyncable {
        byte[] Rawdata{ get; }
    }
}
