using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSimulator : MonoBehaviour
{
	public PlayerController playerController;
	public PathGen pathGen;
	public PathFitter pathFitter;
    public PathFitter pathVis;

    // Use this for initialization
    void Start()
	{
		playerController = new PlayerController(new LayerMask(), null, () => null, () => null, () => null, () => null);
		var actions = pathGen.GenerateRhythm(PathGen.Pattern.Regular, PathGen.Density.High, 30);
		SimulateActionList(actions);
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

    void SimulateActionList(List<PathGen.Action> actions)
	{
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
        List<Vector3> fitPoints = new List<Vector3>();

        Queue<Event> queue = new(events);
		while (queue.Count > 0)
		{
			var item = queue.Dequeue();
			// update the player state based on the action
			ApplyMove(item);

			if (queue.TryPeek(out var next))
			{
                // TODO: Visualiser for Tick() function, add callbaack to store transform
                playerController.Tick(item.time, (next.time - item.time), (pos) =>
				{
					pathPoints.Add(pos);

                }); // simulate being in the updated state until the next event
                fitPoints.Add(playerController._transform.position);
            } else
			{
				break;
			}
        }

		pathFitter.path = fitPoints;
        pathVis.path = pathPoints;
    }

	void ApplyMove(Event ev)
    {
		//// apparently pattern matching has to return something (docs say otherwise)
		//System.Action action = ev switch
		//{
  //          (EventVerb.Start, PathGen.Verb.Move) => () => playerController.Move = 1.0f,
  //          (EventVerb.End, PathGen.Verb.Move) => () => playerController.Move = 0.0f,
  //          (EventVerb.Start, PathGen.Verb.Jump) => () => playerController.Jump = true,
  //          (EventVerb.End, PathGen.Verb.Jump) => () => playerController.Jump = false,
		//	_ => () => { }
  //      };

		//action();
    }
}

