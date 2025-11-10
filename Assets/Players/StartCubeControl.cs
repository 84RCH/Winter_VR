using UnityEngine;
using UnityEngine.UIElements;

public class StartCubeControl : MonoBehaviour
{
    public GameMaster Master;

    public Material ColorStart;
    public Material ColorRestart;
    public Material ColorDefault;

    private enum Status
    {
        StartButton,
        ReStartButton,
        UntouchButton,
        Awake
    }

    private Status status = Status.Awake;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ChangeStatus(Status.UntouchButton);
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bat"))
        {
            Debug.Log("inTrigger");
            ChangeStatus(Status.UntouchButton);
        }
    }

    private void ChangeStatus(Status nextStatus)
    {

        if (nextStatus == Status.StartButton)
        {
            Debug.Log("inStart");
            meshRenderer.material = ColorStart;
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if (nextStatus == Status.ReStartButton)
        {
            Debug.Log("inReStart");
            meshRenderer.material = ColorRestart;
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        else if (nextStatus == Status.UntouchButton)
        {
            Debug.Log("inUntouch");
            meshRenderer.material = ColorDefault;
            gameObject.GetComponent<BoxCollider>().enabled = false;

            if (status==Status.StartButton)
            {
                Master.StartGame();
                StartCoroutine(ReChangeStatus(Status.ReStartButton,5f));
            }
            else if(status==Status.ReStartButton)
            {
                Master.ReStartGame();
            }
            else if(status==Status.Awake)
            {
                StartCoroutine(ReChangeStatus(Status.StartButton,3f));
            }
                
        }

        status = nextStatus;
    }

    private System.Collections.IEnumerator ReChangeStatus(Status nextStatus,float sec)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("inWait");
        ChangeStatus(nextStatus);
    }
}
