using GraphVisualizer;
using SkillIssue.CharacterSpace;
using SkillIssue.StateMachineSpace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class CharacterAnimationManager : MonoBehaviour
{
    public string[] animNames;
    Animator animator;

    private Character character;

    PlayableGraph graph;
    private AnimationMixerPlayable mixerPlayable;
    private AnimationClipPlayable movementPlayable;
    private AnimationClipPlayable actionPlayable;
    private ScriptPlayable<ActionPlayableBehaviour> actionScriptPlayable;


    private Dictionary<AnimationClip, AnimationClipPlayable> movementPlayables = new Dictionary<AnimationClip, AnimationClipPlayable>();
    private Dictionary<AnimationClip, AnimationClipPlayable> actionPlayables = new Dictionary<AnimationClip, AnimationClipPlayable>();

    public void Initialize(Character character, Animator animator)
    {
        this.character = character;
        this.animator = animator;

        // Create PlayableGraph
        graph = PlayableGraph.Create("CharacterAnimationGraph"+character.name);

        // Create Animation Mixer with 2 inputs (Movement + Action)
        mixerPlayable = AnimationMixerPlayable.Create(graph, 2);

        // Pre-cache movement animations
        CacheAnimationsPlayable();

        // Connect default movement animation (Idle)
        graph.Connect(movementPlayable, 0, mixerPlayable, 0);
        mixerPlayable.SetInputWeight(0, 1.0f);

        // Create reusable ActionPlayableBehaviour
        actionScriptPlayable = ScriptPlayable<ActionPlayableBehaviour>.Create(graph,1);
        actionScriptPlayable.GetBehaviour().Initialize(this, character);
        // Connect to Mixer (initially disabled)
        graph.Connect(actionScriptPlayable, 0, mixerPlayable, 1);
        mixerPlayable.SetInputWeight(1, 0.0f);

        // Set up Animation Output
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "AnimationOutput", animator);
        output.SetSourcePlayable(mixerPlayable);

        graph.Play();
    }

    private void CacheAnimationsPlayable()
    {

        foreach (var anim in character.GetCharacterMovementClips())
        {
            CacheMovementPlayable(anim);
        }

        foreach (var anim in character.GetCharacterActionClips())
        {
            CacheActionPlayable(anim);
        }

    }

    void CacheMovementPlayable(AnimationClip clip)
    {
        if (!movementPlayables.ContainsKey(clip))
        {
            movementPlayables[clip] = AnimationClipPlayable.Create(graph, clip);
            movementPlayable = movementPlayables[clip];
        }
    }

    void CacheActionPlayable(AnimationClip clip)
    {
        if (!actionPlayables.ContainsKey(clip))
        {
            actionPlayables[clip] = AnimationClipPlayable.Create(graph, clip);
            actionPlayable = actionPlayables[clip];
        }
    }

    public bool IsPlayingActionAnimation()
    {
        return actionScriptPlayable.GetPlayState() == PlayState.Playing;
    }

    public void PauseActionPlayabe()
    {
        if (actionScriptPlayable.GetPlayState() == PlayState.Paused)
            return;
        actionScriptPlayable.Pause();
    }

    public void ResumeActionPlayable()
    {
        if (actionScriptPlayable.GetPlayState() == PlayState.Playing)
            return;
        actionScriptPlayable.Play();
    }

    // Switch Movement Animation using cached playables
    public void ChangeMovementState(AnimationClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogError("No animation assigned");
            return;
        }
        if (!movementPlayables.ContainsKey(newClip)) return;

        graph.Disconnect(mixerPlayable, 0);
        graph.Connect(movementPlayables[newClip], 0, mixerPlayable, 0);
        if(character.GetCurrentActionState() != ActionStates.None)
        {
            mixerPlayable.SetInputWeight(0,0);
        }
    }

    // Play Action Animation (Overrides Movement)
    public void PlayActionAnimation(AnimationClip actionClip)
    {
        if (actionClip == null)
        {
            Debug.LogError("No animation assigned");
            return;
        }

        // Ensure the action clip is cached
        CacheActionPlayable(actionClip);

        // Get cached Action Playable
        actionPlayable = actionPlayables[actionClip];
        actionPlayable.SetDone(false);
        actionPlayable.SetTime(0);
        actionPlayable.SetTime(0);
        actionScriptPlayable.SetTime(0);

        // Reconnect to existing ActionPlayableBehaviour
        graph.Disconnect(actionScriptPlayable, 0);
        graph.Connect(actionPlayable, 0, actionScriptPlayable, 0);

        actionPlayable.SetDuration(actionClip.length);
        mixerPlayable.SetInputWeight(1, 1.0f); // Enable action animation
        mixerPlayable.SetInputWeight(0, 0.0f); // Disable movement animation
        actionScriptPlayable.Play();
    }

    public void SwitchActionAnimation(AnimationClip actionClip)
    {
        if (actionClip == null)
        {
            Debug.LogError("No animation assigned");
            return;
        }
        actionScriptPlayable.GetBehaviour().SetNextClip(actionClip);
    }

    // Restore movement after action animation ends
    public void OnActionAnimationEnd()
    {
        mixerPlayable.SetInputWeight(1, 0.0f);
        mixerPlayable.SetInputWeight(0, 1.0f);
    }

    void OnDestroy()
    {
        graph.Destroy();
    }

}

public class ActionPlayableBehaviour : PlayableBehaviour
{
    private CharacterAnimationManager controller;
    private Character character;
    private AnimationClip nextClip;

    public void Initialize(CharacterAnimationManager controller, Character character)
    {
        this.controller = controller;
        this.character = character;
    }

    public void SetNextClip(AnimationClip clip)
    {
        nextClip = clip;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (nextClip != null)
        {
            controller.PlayActionAnimation(nextClip);
            nextClip = null;
        }
        base.OnBehaviourPlay(playable, info);
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {

        // Ensure we have at least one input
        if (playable.GetInputCount() == 0)
            return;

        Playable inputPlayable = playable.GetInput(0);
        if (!inputPlayable.IsValid())
        {
            if ((character.GetCurrentActionState() == ActionStates.Block || character.GetCurrentActionState() == ActionStates.Hit))
                controller.OnActionAnimationEnd();
            return;
        }

        // Check if animation has finished playing
        if (inputPlayable.IsDone())
        {
            playable.DisconnectInput(0);
        }
    }
}




