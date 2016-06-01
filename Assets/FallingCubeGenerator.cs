using UnityEngine;
using System.Collections;
using System.Linq.Expressions;

public class FallingCubeGenerator : MonoBehaviour {
	private float frame_counter;
	public GameObject cube; //last square dropped
   public GameObject prevCube;
	public GameObject cam;
	private float cam_left_to_move;
	private bool even_cube;
	private float move_by;
	private float curr_height;
   private bool collided;
   private bool good_next_cube;

	// Use this for initialization
	void Start() {
      collided = false;
		frame_counter = cam_left_to_move = 0;
		even_cube = true;
		curr_height = 1.05f;
      move_by = 0.1f;
      good_next_cube = true;

		cube = NewCube();
		Rigidbody rb = cube.AddComponent<Rigidbody>();
		curr_height += 1f;
		cube = NewCube();

	}

   void OnCollide(GameObject old, GameObject cube) {
      Debug.Log("collided", old);
      good_next_cube = true;
      Destroy(old.GetComponent<Rigidbody>(), 1);

      var pos1 = old.transform.position;
      var pos2 = cube.transform.position;

      var diffx = Mathf.Abs(pos1.x - pos2.x);
      var diffz = Mathf.Abs(pos1.z - pos2.z);

      if (diffx > 0.1 || diffz > 0.1) {
         Destroy(old);
         GameObject new_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         new_cube.transform.position = pos1;
         new_cube.transform.localScale = new Vector3(3-diffx, 1, 3-diffz);
         Rigidbody rb = new_cube.AddComponent<Rigidbody>();
         Destroy(rb, 1);
      }
   }

	// Update is called once per frame
	void Update() {
		frame_counter++;

		bool clicked = Input.GetMouseButton(0) && frame_counter > 10;
		if (clicked)
			frame_counter = 0;

      MoveTopCube();

		if (clicked && good_next_cube) {
         good_next_cube = false;
			even_cube = !even_cube;

			cube.AddComponent<Rigidbody>();
			curr_height += 1;

			//if (cam != null)
			cam_left_to_move += 0.9f;
         prevCube = cube;
			cube = NewCube();
         cube.AddComponent<CubeCollider>();
         CubeCollider cb = cube.GetComponent<CubeCollider>();
         collided = false;
         cb.callback = x => {
            if (!collided)
               OnCollide(prevCube, cube); //Debug.Log("test");
            collided = true;
         };

		}

		if (cam_left_to_move > 0) {
			float moveCameraBy = 0.05f;
			cam.transform.position = cam.transform.position + (new Vector3 (0, moveCameraBy, 0));
			cam_left_to_move -= moveCameraBy;
		}
	}

	GameObject NewCube() {
      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      Renderer rend = cube.GetComponent<Renderer>();
      rend.material = Resources.Load("MyMaterial", typeof(Material)) as Material;
      var red = Random.Range(0, 100) / 100.0f;
      var green = Random.Range(0, 50) / 100.0f;
      var blue = Random.Range(50, 100) / 100.0f;
      var color = new Color(red, green, blue, 0);
      rend.material.SetColor("_Color", color);//Color.blue);

		cube.transform.position = new Vector3(0, curr_height, 0);
		cube.transform.localScale = new Vector3(3, 1, 3);
		return cube;
	}

   void MoveTopCube() {
		double max_dist = 3;

      var cube_pos = cube.transform.position;

		if (Mathf.Abs(cube_pos.x) > max_dist || Mathf.Abs(cube_pos.z) > max_dist)
         move_by = -move_by;

      Vector3 cube_move = new Vector3(move_by, 0, 0);
      if (!even_cube) cube_move = new Vector3(0, 0, move_by);

      cube.transform.position += cube_move;
   }

}

public class CubeCollider : MonoBehaviour {
	public delegate void CallBack(Collision col);
   public CallBack callback;

   void OnCollisionEnter(Collision col) {
      callback(col);
   }
}

