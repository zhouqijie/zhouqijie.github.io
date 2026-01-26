using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewController : MonoBehaviour
{
    private Animator animator;
    private GameObject charactor;
    private GameObject disguised;

    public Transform headHold;
    public Transform backHold;
    public Transform weaponHold;


    private bool isinit = false;

    // Start is called before the first frame update
    public void Awake()
    {
        if (!isinit) Init();
    }

    public void Start()
    {
        this.transform.localScale = new Vector3(150f, 150f, 150f);
    }

    private void Init()
    {
        animator = this.GetComponentInChildren<Animator>(true);
        charactor = animator.gameObject;
        disguised = this.transform.Find("Disguised").gameObject;

        InvokeRepeating("ChangeAnim", 1f, 1f);
        isinit = true;
    }


    private void ChangeAnim()
    {
        if (!isinit) Init();

        int rdm = Random.Range(0, 3);

        switch (rdm)
        {
            case 0://run
                animator.SetFloat("Speed_f", 1);
                animator.SetInteger("Animation_int", 0);
                animator.SetBool("Jump_b", false);
                animator.SetBool("Reload_b", false);
                break;
            case 1://jump
                animator.SetFloat("Speed_f", 0);
                animator.SetInteger("Animation_int", 0);
                animator.SetBool("Jump_b", true);
                animator.SetBool("Reload_b", false);
                break;
            case 2://reload
                animator.SetFloat("Speed_f", 0);
                animator.SetInteger("Animation_int", 0);
                animator.SetBool("Jump_b", false);
                animator.SetBool("Reload_b", true);
                break;
            default:
                break;
        }

    }

    public void ResetParent(Transform parentTrans)
    {
        if (!isinit) Init();

        this.transform.parent = parentTrans;
        this.transform.localPosition = new Vector3();
        ResetCharactor();
    }

    public void ResetCharactor(bool keepSkinTry = false, bool keepDecoTry = false)
    {
        Debug.Log("RetSetCharacter.. keepDecoTry?:  " + keepDecoTry);

        if (!isinit) Init();

        //SKIN
        if (!keepSkinTry)
        {
            foreach (SkinnedMeshRenderer smr in this.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr.gameObject.name == Infomanager.GetInstance().userData.activeSkin.name)
                {
                    smr.gameObject.SetActive(true);
                }
                else
                {
                    smr.gameObject.SetActive(false);
                }
            }
        }





        //DECO 
        if (!keepDecoTry)
        {
            if (headHold.childCount > 0) Destroy(headHold.GetChild(0).gameObject);
            if (backHold.childCount > 0) Destroy(backHold.GetChild(0).gameObject);
            if (weaponHold.childCount > 0) Destroy(weaponHold.GetChild(0).gameObject);

            if (Infomanager.GetInstance().userData.activeDecoration0 != null)
            {
                string str0 = Infomanager.GetInstance().userData.activeDecoration0.name;
                GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + str0), headHold.transform);
                deco.transform.localPosition = new Vector3();
                deco.transform.localEulerAngles = new Vector3();
            }

            if (Infomanager.GetInstance().userData.activeDecoration1 != null)
            {
                string str1 = Infomanager.GetInstance().userData.activeDecoration1.name;
                GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + str1), backHold.transform);
                deco.transform.localPosition = new Vector3();
                deco.transform.localEulerAngles = new Vector3();
            }

            if (Infomanager.GetInstance().userData.activeDecoration2 != null)
            {
                string str2 = Infomanager.GetInstance().userData.activeDecoration2.name;
                GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + str2), weaponHold.transform);
                deco.transform.localPosition = new Vector3();
                deco.transform.localEulerAngles = new Vector3();
            }
        }
        


        //WeaponType
        if (Infomanager.GetInstance().userData.activeDecoration2 != null)
        {
            animator.SetInteger("WeaponType_int", 2);
        }
        else
        {
            animator.SetInteger("WeaponType_int", 0);
        }
    }






    public void TryDeco(SkinInfo skin)
    {
        if (Infomanager.GetInstance().userData.activeSkin == skin) return;

        foreach (SkinnedMeshRenderer smr in this.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            
            if (smr.gameObject.name == skin.name)
            {
                smr.gameObject.SetActive(true);
            }
            else
            {
                smr.gameObject.SetActive(false);
            }
        }
    }

    public void TryDeco(DecorationInfo decoration)
    {
        if (Infomanager.GetInstance().userData.activeDecoration0 == decoration || Infomanager.GetInstance().userData.activeDecoration1 == decoration || Infomanager.GetInstance().userData.activeDecoration2 == decoration) return;
        
        switch (decoration.group)
        {
            case "head":
                {
                    if (headHold.childCount > 0) DestroyImmediate(headHold.GetChild(0).gameObject);
                    
                    GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + decoration.name), headHold.transform);
                    deco.transform.localPosition = new Vector3();
                    deco.transform.localEulerAngles = new Vector3();
                }
                break;
            case "back":
                {
                    if (backHold.childCount > 0) DestroyImmediate(backHold.GetChild(0).gameObject);

                    GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + decoration.name), backHold.transform);
                    deco.transform.localPosition = new Vector3();
                    deco.transform.localEulerAngles = new Vector3();
                }
                break;
            case "weapon":
                {
                    if (weaponHold.childCount > 0) DestroyImmediate(weaponHold.GetChild(0).gameObject);

                    GameObject deco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + decoration.name), weaponHold.transform);
                    deco.transform.localPosition = new Vector3();
                    deco.transform.localEulerAngles = new Vector3();

                    //WeaponType
                    if (deco != null)
                    {
                        animator.SetInteger("WeaponType_int", 2);
                    }
                    else
                    {
                        animator.SetInteger("WeaponType_int", 0);
                    }
                }
                break;
            default:
                break;
        }
    }







    public static PreviewController Create(Transform parent)
    {
        GameObject objPreview = Instantiate(Resources.Load<GameObject>("Prefabs/Charactor"), new Vector3(), new Quaternion(), parent);
        objPreview.AddComponent<PreviewController>();
        objPreview.GetComponent<PreviewController>().headHold = objPreview.GetComponent<CharactorController>().headHold;
        objPreview.GetComponent<PreviewController>().backHold = objPreview.GetComponent<CharactorController>().backHold;
        objPreview.GetComponent<PreviewController>().weaponHold = objPreview.GetComponent<CharactorController>().weaponHold;
        DestroyImmediate(objPreview.GetComponent<CharactorController>());
        DestroyImmediate(objPreview.GetComponent<UnityEngine.AI.NavMeshAgent>());
        DestroyImmediate(objPreview.GetComponent<Rigidbody>());

        objPreview.transform.Find("Sounds").gameObject.SetActive(false);

        objPreview.transform.localScale = new Vector3(100, 100, 100);
        objPreview.transform.localPosition = new Vector3(0, 0, 0);
        objPreview.transform.localEulerAngles = new Vector3(0f, 150f, 0f);

        
        return objPreview.GetComponent<PreviewController>();
    }
}
