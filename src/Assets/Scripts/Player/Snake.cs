using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using UnityEngine.InputSystem;

namespace Snake
{
	[AddComponentMenu("SNAKE/Snake")]
	[RequireComponent (typeof (BoxCollider2D))]
	[RequireComponent (typeof (Rigidbody2D))]
	public class Snake : MonoBehaviour 
	{
		private readonly string m_itemTag = "Item";
		private readonly float m_fasterMoveSpeed = .1f;

		private IInputController m_inputController;
		private Vector2 m_direction;
		private float m_startMoveSpeed;
		private List<Transform> m_tail;
		private Coroutine m_moveCoroutine;
		private NumberItem m_numberItem;
		private bool m_isReversing;

		[SerializeField] private TailSpawner m_tailSpawner;
		[Range(0f, 2f)] [SerializeField] private float m_moveSpeed;
		[Range(0f, 1f)] [SerializeField] private float m_incrementSpeed;
		[Range(0f, 10f)] [SerializeField] private float m_bonusSlowdown;

		public static SnakeData Data;

        public bool isinReplay;
        public float current;
        public float indexchangerate = 0;
        public List<RecordReplay> actionReplays = new List<RecordReplay>();
		public List<ActionReplay> tails = new List<ActionReplay>();
		//public List<Replay> replays;
        public Rigidbody2D rb;
		public Sprite[] tail;

		public Controls input;
		public Vector2 moveDir;
		public GameObject gameover;
		public GameObject[] letters;
		public AudioSource audioSource;
		public AudioClip[] audioClip;

        //[System.Serializable]
        //public class Replay
        //{
        //    public List<RecordReplay> replays = new List<RecordReplay>();
        //}

		public SwipeListener swipeListener;

        private void OnEnable()
        {
			swipeListener.OnSwipe.AddListener(Swiping);
			audioSource = GetComponent<AudioSource>();
			input.Enable();
        }

        private void OnDisable()
        {
			swipeListener.OnSwipe.RemoveListener(Swiping);
			input.Disable();
        }


        private void Awake ()
		{
			Data = new SnakeData ();
			input = new Controls ();
		}

		private void Start ()
		{

			m_inputController = InputFactory.GetCurrentInputController ();
			m_direction = Vector2.right;
			m_startMoveSpeed = m_moveSpeed;
			m_tail = new List<Transform> ();

			GameUI.Instance.UpdateLength (1);

            rb = GetComponent<Rigidbody2D>();

            m_moveCoroutine = StartCoroutine (Move ());
			actionReplays.Capacity = 150;
            //StartCoroutine(Rewind());
        }

		private void Update ()
		{
			m_direction = m_inputController.GetDirectionValue (m_direction);

			//        if (isinReplay)
			//        {
			////SetTransform(0);
			//StartCoroutine(StopRewind());
			//        }
        }

        private void FixedUpdate()
        {
            if(moveDir.x < 0 )
			{
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[1];
				print("hi");
            }
			else if(moveDir.x > 0)
			{
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[0];
            }
			else if(moveDir.y < 0)
			{
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[3];
            }
			else if(moveDir.y> 0)
			{
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[2];
            }
        }

        public void Move(InputAction.CallbackContext callback)
        {
            moveDir = callback.ReadValue<Vector2>();
        }

        IEnumerator StopRewind()
		{
			yield return new WaitForSeconds(5);
			indexchangerate = 0;
			isinReplay = false;
			StopCoroutine(StopRewind ());
		}


		IEnumerator Rewind()
		{
			

            while (isinReplay == false)
            {
                yield return new WaitForSeconds(1f / 30);
                if (actionReplays.Count == 100)
				{
					actionReplays.Remove(actionReplays[0]);
				}
                actionReplays.Add(new RecordReplay { pos = transform.position, rot = transform.rotation });
                for(int i = 0;i < tails.Count;i++)
				{
					tails[i].recording = true;
					tails[i].StartCoroutine(tails[i].SetRecorder());
                }
                //for(int i = 0; i <= tail.Length; i++)
                //{
                //	if(replays[i].replays.Count <= tail.Length)
                //	{
                //		replays.Add(new Replay());

                //	}
                //                if (replays.Count == 100)
                //                {

                //                    replays.Remove(replays[0]);

                //                }

                //	replays.Add(new Replay { pos = transform.position, rot = transform.rotation });
                //            }

                //StartCoroutine(Rewind());

            }
			//float nextIndex = current + indexchangerate;

			for (int i = actionReplays.Count - 1; i >= 0; i--)//(nextIndex < actionReplays.Count && nextIndex >= 0)
			{

				yield return new WaitForSeconds(1f / 30);
				SetTransform(i);

				//SetTransformTail ();
				for (int j = 0; j < tails.Count; j++)
				{
					tails[j].recording = false;
                    if (tails[j].Recorder.Count < i)
                    {
						tails[j].gameObject.SetActive(false);
                        tails.Remove(tails[j]);

                    }
					else
					{
                        tails[j].Playback(i);
                        tails[j].StopCoroutine(tails[j].SetRecorder());
                    }

                }

			}


            //for (int i = 0; i < tail.Length; i++)
            //{
            //	for (int j = replays[i].replays.Count - 1; j >= 0; j--)
            //	{
            //		SetTransformTail(j);
            //	}
            //}
            isinReplay = false;
			StartCoroutine (Rewind());
			yield return new WaitForSeconds(1);
            m_moveCoroutine = StartCoroutine(Move());


        }

        private void SetTransform(float index)
        {
            current = index;
            RecordReplay record = actionReplays[(int)index];

            transform.position = record.pos;
            transform.rotation = record.rot;
        }

		//private void SetTransformTail()
		//{
  //          foreach (var replay in tails)
  //          {
  //              replay.recording = false;
  //              replay.StartCoroutine(replay.Playback());
  //          }
  //      }

		private void OnTriggerEnter2D (Collider2D collider)
		{
			if (collider.CompareTag (m_itemTag))
			{
				SoundManager.Instance.PlaySoundEffect ("Item Pickup");

				var item = collider.GetComponent <Item> ();

				ExecuteItemBehaviour (item);
                audioSource.PlayOneShot(audioClip[1]);
            }
			else 
			{
				Stop ();
				audioSource.PlayOneShot(audioClip[0]);
                //indexchangerate = -1;
                //isinReplay = true;
                GameManager.Instance.GameOver ();
				gameover.SetActive (false);
				
            }
		}
			
		private IEnumerator Move ()
		{
			var sfxMove = new string[] { "Move1 Blip", "Move2 Blip" };

			while (true)
			{
				yield return new WaitForSeconds (m_moveSpeed);

				if (m_isReversing) 
				{
					ReverseSnakeHead ();

					m_isReversing = false;
				} 
				else 
				{
					var currentPosition = transform.localPosition;

					SoundManager.Instance.PlaySoundEffect (sfxMove [Random.Range (0, sfxMove.Length)]);
					transform.Translate (m_direction);

					if (m_numberItem != null) 
					{
						IncreaseTailAndSpeed (currentPosition);

						m_numberItem = null;
					} 
					else if (m_tail.Count > 0) 
					{
						m_tail.Last ().localPosition = currentPosition;

						m_tail.Insert (0, m_tail.Last ());
						m_tail.RemoveAt (m_tail.Count - 1);
					}
				}
			}
		}

		private void ExecuteItemBehaviour (Item item)
		{
			item.Hide ();

			switch (item.Kind)
			{
				case ItemKind.Number:
					m_numberItem = item as NumberItem;
					//tail = GameObject.FindGameObjectsWithTag("Tail");
					//for(int i = 0; i < tail.Length; i++)
					//{
					//	replays.Add(new Replay());
					//}
					break;
				case ItemKind.Bonus:
					SlowdownSpeedTemporarily ();
					break;
				case ItemKind.Reverse:
					m_isReversing = true;
					break;
				case ItemKind.Alphabet:
					for(int i = 0; i < letters.Length; i++)
					{
						if (letters[i].activeInHierarchy == false)
						{
							letters[i].SetActive(true);
							return;
						}
					}
					break;
			}
		}

		private void IncreaseTailAndSpeed (Vector2 currentPosition)
		{
			int value = m_numberItem.Value;
			GameUI.Instance.UpdateLength (value);

			for (int i = 0; i < value; i++)
			{
				var newTail = m_tailSpawner.Spawn (currentPosition);
				//ActionReplay tail = newTail.GetComponent<ActionReplay>();

                //tails.Add(tail);

				//tail.StartCoroutine(tail.SetRecorder());
				m_tail.Insert (0, newTail);
			}

			if (!GameUI.Instance.IsBonusItemImageEnabled)
			{
				m_moveSpeed -= m_incrementSpeed;
				m_moveSpeed = Mathf.Clamp (m_moveSpeed, m_fasterMoveSpeed, m_startMoveSpeed);
			}
		}

		private void SlowdownSpeedTemporarily ()
		{
			GameUI.Instance.UpdateBonusImage (true);

			var currentMoveSpeed = m_moveSpeed;
			m_moveSpeed = m_startMoveSpeed;

			StartCoroutine (RestoreSpeed (currentMoveSpeed));
		}

		private IEnumerator RestoreSpeed (float lastMoveSpeed)
		{
			yield return new WaitForSeconds (m_bonusSlowdown);

			GameUI.Instance.UpdateBonusImage (false);

			m_moveSpeed = lastMoveSpeed;
		}

		private void ReverseSnakeHead ()
		{
			if (m_tail.Count == 0) 
			{
				return;
			}

			var currentPosition = transform.localPosition;
			transform.localPosition = m_tail.Last ().localPosition;
			m_tail.Last ().localPosition = currentPosition;

			m_tail.Insert (0, m_tail.Last ());
			m_tail.RemoveAt (m_tail.Count - 1);
			m_tail.Reverse ();

			if (m_direction == Vector2.right)
			{
				EvaluateDirectionWithoutCollision (Vector2.left, Vector2.up, Vector2.down);
            }
			else if (m_direction == Vector2.left)
			{
				EvaluateDirectionWithoutCollision (Vector2.right, Vector2.up, Vector2.down);
            }
			else if (m_direction == Vector2.up)
			{
				EvaluateDirectionWithoutCollision (Vector2.down, Vector2.right, Vector2.left);
            }
			else if (m_direction == Vector2.down)
			{
				EvaluateDirectionWithoutCollision (Vector2.up, Vector2.right, Vector2.left);
            }
		}

		public void Swiping(string swipe)
		{
			if(swipe == "Left")
			{
                EvaluateDirectionWithoutCollision(Vector2.left, Vector2.up, Vector2.down);
				gameObject.GetComponent<SpriteRenderer>().sprite = tail[1];
            }
			else if(swipe == "Right")
			{
                EvaluateDirectionWithoutCollision(Vector2.right, Vector2.up, Vector2.down);
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[0];
            }
			else if(swipe == "Up")
            {
                EvaluateDirectionWithoutCollision(Vector2.up, Vector2.right, Vector2.left);
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[2];
            }
			else if(swipe == "Down")
			{
                EvaluateDirectionWithoutCollision(Vector2.down, Vector2.right, Vector2.left);
                gameObject.GetComponent<SpriteRenderer>().sprite = tail[3];
            }
		}

		private void EvaluateDirectionWithoutCollision (params Vector2[] directionOptions)
		{
			Vector3 direction = directionOptions.First ();

			for (int i = 0; i < directionOptions.Length; i++)
			{
				bool hasCollision = false;

				for (int l = 0; l < m_tail.Count; l++)
				{
					if (transform.localPosition + direction == m_tail[l].localPosition) 
					{
						hasCollision = true;

						break;
					}
				}

				if (!hasCollision)
				{
					break;
				}

				direction = directionOptions [i];
			}

			m_direction = direction;
		}

		private void Stop ()
		{
			StopCoroutine (m_moveCoroutine);
		}
	}
}