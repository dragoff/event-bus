using EventBusSpace;
using UnityEngine;

class MyBus : EventBus
{
    [LogEvent]
    public static BusEvent EventZero;
    public static BusEvent<string> EventOne;
    public static BusState<int> StateInt;
    static MyBus() => InitFields<MyBus>();
}

public class SampleScript : MonoBehaviour
{
    private void Start()
    {
        MyBus.EventOne.Subscribe(this, s => Debug.Log($"MyBus.EventOne happened: {s}"));
        MyBus.EventZero.Subscribe(this, () => Debug.Log($"MyBus.EventZero happened"));
        MyBus.StateInt.Subscribe(this, () => Debug.Log($"MyBus.StateInt changed: {MyBus.StateInt.Value}"));

        MyBus.EventOne += "test";
        MyBus.EventZero += true;
        MyBus.StateInt.Value = int.MaxValue;
    }
}
