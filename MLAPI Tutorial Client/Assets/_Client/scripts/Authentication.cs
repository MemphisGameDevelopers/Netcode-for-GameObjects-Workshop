using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class Authentication : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        print($"Unity Services: {UnityServices.State}");
        await SignInAnonymouslyAsync();
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
             await AuthenticationService.Instance.SignInAnonymouslyAsync();
             print("Anonymous Sign In Successful!");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException reqEx)
        {
            Debug.LogException(reqEx);
        }
    }
    
}
