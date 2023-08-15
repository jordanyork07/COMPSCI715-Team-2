using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSimulator : MonoBehaviour
{
	public PlayerController playerController = new();
	public PathGen pathGen;
	public PathFitter pathFitter;

	// Use this for initialization
	void Start()
	{
		var actions = pathGen.GenerateRhythm(PathGen.Pattern.Regular, PathGen.Density.Medium, 20);
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
		List<Vector3> pathPoints = new List<Vector3>();

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

		Queue<Event> queue = new(events);
		while (queue.Count > 0)
		{
			var item = queue.Dequeue();
			ApplyMove(item);

			var next = queue.Peek();

			// TODO: Visualiser for Tick() function, add callbaack to store transform
			playerController.Tick(next.time - item.time);
			pathPoints.Add(playerController.transform.position);
        }

		pathFitter.path = pathPoints;
    }

	void ApplyMove(Event ev)
    {
		System.Action action = ev switch
		{
            (EventVerb.Start, PathGen.Verb.Move) => () => playerController.Move = 1.0f,
            (EventVerb.End, PathGen.Verb.Move) => () => playerController.Move = 0.0f,
            (EventVerb.Start, PathGen.Verb.Jump) => () => playerController.Jump = true,
            (EventVerb.End, PathGen.Verb.Jump) => () => playerController.Jump = false,
			_ => () => { }
        };

		action();
    }
}

