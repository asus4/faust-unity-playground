using UnityEngine;
using FaustUtilities_SimpleOsc;

/* @brief How To Use
* Attached this script to a game object.
* It will create an Audio source, attach the dsp to it and display the plugin interface in the inspector
*/

/* @brief This class supports parameter access in the plugin
* In the plugin, each parameter is accessed using an ID.
* IDs are integers starting at 0. They are assigned following the order they have been implemented in the Faust program.
* To access the parameter, use the GetParameter() and SetParameter() methods, passing the parameter ID.
* IDs are displayed as tooltips in the inspector, mouse over the parameter (e.g., a slider) to display the tooltip.
* Passive parameters (bargraphs) cannot be set.
*/

[RequireComponent(typeof(AudioSource))] // Automatically attached the script to an Audiosource
public class FaustPlugin_SimpleOsc: MonoBehaviour {
	// Array containing the value of each parameter, has to be public to be serialized by the Unity engine
	public float[] parameters = new float[32];
	private Faust_Context ctx;

	// Initializes the interface between the plugin and Unity
	public Faust_Context context {
	    get {
	       return ctx;
        }
    }

	/* @brief Returns true if the plugin is instantiated (the plugin is instantiated when play mode button is pressed)
    */
	public bool IsInstantiated() {
		return (ctx != null);
	}

	/* @brief Gets a parameter value from the plugin
    * @param param Use the parameter number available in the parameter inspector (tooltip)
    * @return The parameter value */
	public float getParameter(int param) {
		if (IsInstantiated()) { // if the the plugin is instantiated, the parameter value is changed directly in the plugin
      		return ctx.getParameterValue(param);
		} else { // if not, the value is stored in parameters[]
    		return parameters[param];
		}
	}

	/* @brief Sets a parameter value in the plugin
    * @param param Use the parameter number available in the parameter inspector (tooltip)
    * @param x New parameter value */
	public void setParameter(int param, float x) {
		if (IsInstantiated()) {
			ctx.setParameterValue(param, x);
		} else {
			parameters[param] = x;
		}
	}

	/* @brief Instantiates the plugin and the interface between the plugin and Unity
    * @brief And sets the new parameter values changed while in pause mode */
	private void Awake() {
		ctx = new Faust_Context(getBufferSize());
		ctx.context_init(AudioSettings.outputSampleRate);
		for (int i = 0; i < parameters.Length; i++) {
			setParameter(i, parameters[i]);
		}
	}

	/* @brief Fills the Unity audio buffer with the audio data computed by the plugin
    */
	private void OnAudioFilterRead(float[] buffer, int numchannels) {
		ctx.process(buffer, buffer.Length / numchannels, numchannels);
	}

	private int getBufferSize() {
		int bufferlength,
		numbuffers;
		AudioSettings.GetDSPBufferSize(out bufferlength, out numbuffers);
		return bufferlength;
	}

	/* @brief Gets the min value of a parameter
    * @param Use the parameter number available in the parameter inspector (tooltip) */
	public float getParameterMin(int param) {
		return ctx.getParamMin(param);
	}

	/* @brief Gets the max value of a parameter
    * @param Use the parameter number available in the parameter inspector (tooltip) */
	public float getParameterMax(int param) {
		return ctx.getParamMax(param);
	}

}
