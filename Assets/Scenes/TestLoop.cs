using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TestLoop : MonoBehaviour
{
    private CancellationTokenSource  _tokenSource = new();
    private void Start()
    {
        F();
    }

    private async Task F()
    {
        var token = _tokenSource.Token;
        var i = 0;
        while (true)
        {
            Debug.Log(i++);
            await Task.Yield();
            
            if (token.IsCancellationRequested)
                break;
        }
    }

    private void OnDestroy()
    {
        throw new NotImplementedException();
    }
}
