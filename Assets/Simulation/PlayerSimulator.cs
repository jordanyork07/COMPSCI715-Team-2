using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static PlayerController;
using System;
using System.Linq;
using Simulation;
using UnityEditor;
using static PathGen;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph;

[CustomEditor(typeof(PlayerSimulator))]
public class PlayerSimulatorEditor : Editor
{
	private PlayerSimulator playerSimulator;

    private void OnEnable()
    {
        playerSimulator = (PlayerSimulator)target;
    }


    public override void OnInspectorGUI()
    {
		base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
	        var actions = playerSimulator.GeneratePath();
            playerSimulator.SimulateActionList(actions);
        }
    }
}

[RequireComponent(typeof(PathGen))]
public class PlayerSimulator : MonoBehaviour
{
	public PlayerController playerController;
	public PathFitter pathFitter;
    public PathFitter pathVis;
	public GameObject simObjectPrefab;

	private InputState inputState = new InputState();
	private InputDelegate GetInputState;

	public List<PathGen.Action> GeneratePath()
	{
		return GetComponent<PathGen>().GetRhythm();
	}

    // Use this for initialization
    void Start()
	{
		
	}

    // Update is called once per frame
    void Update()
	{

	}

    enum EventVerb
	{
		Start,
		End
	}

	record Event
	{
		public float time;
        public EventVerb verb;
        public PathGen.Action action;

        public void Deconstruct(out EventVerb eventVerb, out PathGen.Verb actionVerb)
        {
			eventVerb = verb;
			actionVerb = action.verb;
        }
    }

    internal void SimulateActionList(List<PathGen.Action> actions)
	{
        var simObject = Instantiate(simObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        inputState.forSimulationSignalGrounded = true;
        GetInputState = () => { return inputState; };
        playerController = new PlayerController(new LayerMask(), () => simObject.transform, () => null, () => null, GetInputState, () => simObject.GetComponent<CharacterController>());

        List<Event> events = new();
        foreach (var action in actions)
		{
			events.Add(new Event()
			{
				time = action.startTime,
				verb = EventVerb.Start,
				action = action
			});
            events.Add(new Event()
            {
                time = action.startTime + action.duration,
                verb = EventVerb.End,
                action = action
            });
		}

		events.Sort((p, q) => p.time.CompareTo(q.time));

        List<Vector3> pathPoints = new List<Vector3>();
        RleList fitPoints = new RleList();

        playerController.IsSimulation = true;
		playerController.Start();
		
		// fitPoints.StartAt(new Vector3(0, 0, 0));

        Queue<Event> queue = new(events);
		while (queue.Count > 0)
		{
			var item = queue.Dequeue();
			// update the player state based on the action
			ApplyMove(item, playerController);

			if (queue.TryPeek(out var next))
			{
                // TODO: Visualiser for Tick() function, add callback to store transform
                playerController.Tick(item.time, (next.time - item.time), (pos) =>
				{
					// if (playerController.Grounded)
					// 	pathPoints.Add(new Vector3(pos.x + 5.0f, pos.y, pos.z));
					// else
					pathPoints.Add(pos);
					
					if (playerController.Grounded)
						fitPoints.StartAt(pos);

                }, true); // simulate being in the updated state until the next event
                
                // Fitting logic (TODO: Extract out)
                var point = playerController.Transform().position;
                
                // Start of Jump (why?)
                // if (playerController.Grounded)
	               //  fitPoints.StartAt(point);
	               
	            
			} else
			{
				break;
			}
        }

		pathFitter.path = fitPoints.GetPoints();
        pathVis.path = pathPoints;

		DestroyImmediate(simObject);
		playerController = null;
    }

	void ApplyMove(Event ev, PlayerController controller)
    {
        // apparently pattern matching has to return something (docs say otherwise)
        inputState.forSimulationSignalGrounded = false;
        System.Action action = ev switch
		{
			(EventVerb.Start, PathGen.Verb.Move) => () =>
			{
				inputState.move = new Vector2(1, 1);
            },
			(EventVerb.End, PathGen.Verb.Move) => () =>
			{
                inputState.move = new Vector2(0, 0);
            },
			(EventVerb.Start, PathGen.Verb.Jump) => () =>
			{
                inputState.jump = true;
            },
			(EventVerb.End, PathGen.Verb.Jump) => () =>
			{
                inputState.jump = false;
                inputState.forSimulationSignalGrounded = true;
			},
			_ => () => { }
		};

		action();
	}
}

