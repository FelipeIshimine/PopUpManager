using System;
using System.Collections.Generic;

public interface IPlayAnimation
{
     Action OnStart { get; set; }
     Action OnEnd { get; set; }
     void Play();
}