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
        graph = PlayableGraph.Create("CharacterAnimationGraph");

        // Create Animation Mixer with 2 inputs (Movement + Action)
        mixerPlayable = AnimationMixerPlayable.Create(graph, 2);

        // Pre-cache movement animations
        CacheAnimationsPlayable();

        // Connect default movement animation (Idle)
        graph.Connect(movementPlayable, 0, mixerPlayable, 0);
        mixerPlayable.SetInputWeight(0, 1.0f);

        // Create reusable ActionPlayableBehaviour
        actionScriptPlayable = ScriptPlayable<ActionPlayableBehaviour>.Create(graph);
        actionScriptPlayable.GetBehaviour().Initialize(this);

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
        }
    }

    void CacheActionPlayable(AnimationClip clip)
    {
        if (!actionPlayables.ContainsKey(clip))
        {
            actionPlayables[clip] = AnimationClipPlayable.Create(graph, clip);
        }
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
        mixerPlayable.SetInputWeight(0, 1.0f);
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

        // Reconnect to existing ActionPlayableBehaviour
        graph.Disconnect(mixerPlayable, 1);
        graph.Connect(actionPlayable, 0, mixerPlayable, 1);

        mixerPlayable.SetInputWeight(1, 1.0f); // Enable action animation
        mixerPlayable.SetInputWeight(0, 0.0f); // Disable movement animation
    }

    // Restore movement after action animation ends
    public void OnActionAnimationEnd()
    {
        Debug.Log("AnimEnd");
        mixerPlayable.SetInputWeight(1, 0.0f);
        mixerPlayable.SetInputWeight(0, 1.0f);
        character.SetActionState(ActionStates.None);
    }

    void OnDestroy()
    {
        graph.Destroy();
    }

}

public class ActionPlayableBehaviour : PlayableBehaviour
{
    private CharacterAnimationManager controller;


    public void Initialize(CharacterAnimationManager controller)
    {
        this.controller = controller;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (playable.IsDone())
        {
            controller.OnActionAnimationEnd();
        }
    }
}




