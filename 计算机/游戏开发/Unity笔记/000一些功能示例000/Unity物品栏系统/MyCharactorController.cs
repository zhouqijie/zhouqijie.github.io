using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharactorController : MonoBehaviour {

	private Camera cam;
	[SerializeField]private GameObject hand;
	[SerializeField]private Inventory inventory;
	[SerializeField]public GameObject currentItem;
	private bool isStopping;
	private bool isGround;





    public GameObject Hand { get { return hand; } }






	void Start () {
		cam = this.GetComponentInChildren<Camera>();
		currentItem = null;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		isStopping = false;
	}
	

	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)){
			if(isStopping){
				//停止变活动
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				isStopping = false;
				inventory.Hide();
			}
			else{
				//活动变停止
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				isStopping = true;
				inventory.Show();
			}
			
		}



        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hitInfo;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 4f);
			
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Item"))
                {
                    GameObject obj = hitInfo.collider.gameObject;
					
                    if(obj.GetComponent<IItem>().Type == ItemType.Large){
						inventory.StoreCurrent();
						currentItem = obj;
						currentItem.GetComponent<IItem>().SetHeld();
						currentItem.transform.SetParent(hand.transform);
						currentItem.transform.position = hand.transform.position;
						currentItem.transform.rotation = hand.transform.rotation;

					}
					else if(obj.GetComponent<IItem>().Type != ItemType.Large){
						this.GetComponentInChildren<Inventory>().Add(obj.GetComponent<IItem>());
					}
                }
            }

        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            AbandonCurrent();
        }


		if(Input.GetKeyDown(KeyCode.Mouse0)){
			if(currentItem != null){
				currentItem.GetComponent<IItem>().Action0();
				inventory.CheckCount();
			}
		}
    }
	
	void FixedUpdate(){
		RaycastHit hitInfo;
		bool castOnGround = Physics.SphereCast(this.transform.position, 0.5f, -this.transform.up, out hitInfo, 0.55f);
		Vector3 normal = hitInfo.normal;
		if(castOnGround){
			isGround = true;
		}
		else{
			isGround = false;
		}

		float inputVertical = Input.GetAxis("Vertical");
		float inputHorizontal = Input.GetAxis("Horizontal");
		float inputMouseX = Input.GetAxis("Mouse X");
		float inputMouseY = Input.GetAxis("Mouse Y");
		bool inputJump = Input.GetKeyDown(KeyCode.Space);

		if(!isStopping){
			this.transform.Rotate(new Vector3(0, 1, 0), inputMouseX);
			cam.transform.Rotate(new Vector3(1, 0, 0), -inputMouseY, Space.Self);
		}
		

		if(isGround){
			this.GetComponent<Rigidbody>().drag = 1f;
			Vector3 forceForward = Vector3.ProjectOnPlane(this.transform.forward * 20f * inputVertical, normal);
			Vector3 forceSideway = Vector3.ProjectOnPlane(this.transform.right * 20f * inputHorizontal, normal);
			this.GetComponent<Rigidbody>().AddForce(forceForward, ForceMode.Impulse);
			this.GetComponent<Rigidbody>().AddForce(forceSideway, ForceMode.Impulse);

			if(inputJump){
				this.GetComponent<Rigidbody>().AddForce(this.transform.up * 60f, ForceMode.Impulse);
			}
		}
		else{
			this.GetComponent<Rigidbody>().drag = 0.1f;
		}
		
	}


	public void AbandonCurrent(){
        if (currentItem != null)
        {
            if (currentItem.GetComponent<IItem>().Type == ItemType.Large)
            {
                GameObject obj_abandon = currentItem;
                currentItem = null;
                obj_abandon.transform.parent = null;
                obj_abandon.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
                obj_abandon.GetComponent<IItem>().SetFree();
                obj_abandon.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 150f * obj_abandon.GetComponent<Rigidbody>().mass);
            }
            else if (currentItem.GetComponent<IItem>().Type != ItemType.Large)
            {
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] != null)
                    {
                        if (inventory[i].GameObj == currentItem)
                        {
                            inventory.Abandon(i);
                            currentItem = null;
                        }
                    }
                }
                inventory.DisplayGrids();
            }
        }

    }
}
