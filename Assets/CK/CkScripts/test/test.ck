#include "../UnityHelper.ckh"
#include "../GameCoreHelper.ckh"



void main()
{
    Print("main start");
    Print(DeltaTime);
    Print("main end");
}



Vector3 Move(Vector3 from, Vector3 to, float time)
{
	Vector3 result = from;
	float rate = 1.0f / time;
	float currentValue = 0.0f;
	while(currentValue < 1.0f)
	{
		yield result;
		float tempValue = 0.0f;
		result = from + (to - from) * currentValue;
		Print(currentValue);
		currentValue += rate * DeltaTime;
	}
	return to;
}


