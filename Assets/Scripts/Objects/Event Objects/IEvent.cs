using System;
using UnityEngine;

public interface IEvent
{
    public enum TYPE { STORY, TUTORIAL, IN_GAME, MAP_CHANGE, LOADING };
    public TYPE EventType { get; set; }
    public IEvent NextEvent { get; set; }
}

