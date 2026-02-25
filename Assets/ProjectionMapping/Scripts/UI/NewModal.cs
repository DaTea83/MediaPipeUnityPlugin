using Mediapipe.Unity.Sample.UI;
using UnityEngine;

namespace ProjectionMapping
{
    public class NewModal : Modal
    {
	    [SerializeField] private HandSolution handSolution;
	    
	    public override void OpenAndPause(GameObject contents)
	    {
		    Open(contents);
		    handSolution?.Pause();
	    }

	    public override void CloseAndResume(bool forceRestart = false)
	    {
		    Close();
		    if(handSolution is null) return;
		    if (forceRestart)
		    {
			    handSolution?.Play();
		    }
		    else
		    {
			    handSolution?.Resume();
		    }
	    }
    }
}
