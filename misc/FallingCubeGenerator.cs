using UnityEngine;
using System.Collections;

public class FallingCubeGenerator : MonoBehaviour {
	private float frame_counter;
	public GameObject cube; //last square dropped
	public GameObject cam;
	private float cam_left_to_move;
	private bool even_cube;
	private bool forward;
	private float curr_height;


	// Use this for initialization
	void Start() {
		frame_counter = cam_left_to_move = 0;
		even_cube = forward = true;
		curr_height = 1f;

		cube = NewCube();
		Rigidbody rb = cube.AddComponent<Rigidbody>();
		curr_height += 0.9f;
		cube = NewCube();

	}
	
	// Update is called once per frame
	void Update() {
		frame_counter++;

		bool clicked = Input.GetMouseButton(0) && frame_counter > 10;
		if (clicked)
			frame_counter = 0;

		double max_dist = 3;

		if (even_cube && Mathf.Abs(cube.transform.position.z) > max_dist) {
			forward = !forward;
		} else if (!even_cube && Mathf.Abs (cube.transform.position.x) > max_dist) {
			forward = !forward;
		}

		float move_by = forward ? 0.1f : -0.1f;

		if (even_cube) {
			cube.transform.position = cube.transform.position + (new Vector3 (0, 0, move_by));
		} else {
			cube.transform.position = cube.transform.position + (new Vector3 (move_by, 0, 0));
		}

		if (clicked) {
			even_cube = !even_cube;

			cube.AddComponent<Rigidbody>();
			curr_height += 1;

			//if (cam != null) 
			cam_left_to_move += 0.9f;

			cube = NewCube ();
		}

		if (cam_left_to_move > 0) {
			float moveCameraBy = 0.05f;
			cam.transform.position = cam.transform.position + (new Vector3 (0, moveCameraBy, 0));
			cam_left_to_move -= moveCameraBy;
		}
			
	}

	GameObject NewCube() {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.localScale = new Vector3(3, 1, 3);
		cube.transform.position = new Vector3(0, curr_height, 0);

		return cube;
	}
}
