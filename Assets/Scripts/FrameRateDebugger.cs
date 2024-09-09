using TMPro;
using UnityEngine;

public class FrameRateDebugger : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _debugTextComp;

    private readonly float _delayRate = 0.1f;

    private float _nextTick;

    private void Update()
    {
        if (_debugTextComp == null)
        {
            return;
        }

        if (Time.time <= _nextTick)
        {
            return;
        }

        _debugTextComp.text = $"{1f / Time.unscaledDeltaTime:N0} FPS";

        _nextTick = Time.time + _delayRate;
    }

}
