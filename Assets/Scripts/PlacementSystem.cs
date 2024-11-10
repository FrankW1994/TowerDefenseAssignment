using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PlacementSystem : MonoBehaviourPun//, IPunObservable
{
    
    public static event Action<int> OnTowerPlaced;
    private PhotonView pv;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private LayerMask CheckMask;
    [SerializeField] private LayerMask CollideMask;
    private GameObject gameState;
    private GameState gs;
    [SerializeField] private LineRenderer lineRenderer;
    //[SerializeField] private Material validMaterial;
    //[SerializeField] private Material invalidMaterial;      // Material for valid placement
    
    //private Renderer renderer;
    private GameObject CurrentTower;
    private GameObject PreviewTower;
    //private float placementRadius = 5f;
    //private GameObject towerPreview;    // Preview instance
    //private Renderer towerRenderer;
    //private bool canPlace = false;  

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        gameState = GameObject.Find("GameState");
        gs = gameState.GetComponent<GameState>();

    }

    //// Update is called once per frame
    void Update()
    {

        //Debug.Log("Not There Yet!");
        if (pv.IsMine){ 
            //Debug.Log("pv IsMine");
            if(PreviewTower != null && CurrentTower != null){
                //Debug.Log("All Good To Go!");
                Ray cameraRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                
                if(Physics.Raycast(cameraRay, out hitInfo, 100f, CollideMask)){
                    //DrawPolygon(hitInfo.point);
                    PreviewTower.transform.position = hitInfo.point;
                    //Debug.Log("Got position from ray.");

                }
                if(Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject != null){
                    Debug.Log("Click");
                    if(!hitInfo.collider.gameObject.CompareTag("CantPlace"))
                    {
                        Debug.Log("CanPlace");
                        BoxCollider TowerCollider = PreviewTower.gameObject.GetComponent<BoxCollider>();
                        TowerCollider.isTrigger = true;

                        Vector3 Center = PreviewTower.gameObject.transform.position + TowerCollider.center;
                        Vector3 halfExtents = TowerCollider.size / 2;
                        if(!Physics.CheckBox(Center, halfExtents, Quaternion.identity, CheckMask, QueryTriggerInteraction.Ignore)){
                            Debug.Log("CanPlace2");
                            PlaceTower(PreviewTower.transform.position);
                            //TowerCollider.isTrigger = false;


                            //OnTowerPlaced?.Invoke(PreviewTower.GetComponent<Tower>().GetCost());

                            //PreviewTower = null;

                        }

                    }
                
                }
            }    
        }
    }
    private void PlaceTower(Vector3 pos){
         // Remove preview
        //Destroy(PreviewTower);
        // Use an RPC to ensure the tower is instantiated at the correct position for all players
        photonView.RPC("RPC_PlaceTower", RpcTarget.All, pos);
    }
    [PunRPC]
    private void RPC_PlaceTower(Vector3 pos){
        Destroy(PreviewTower);
        CurrentTower.transform.position = pos;
        CurrentTower.SetActive(true);
        //PhotonNetwork.Instantiate(towerName, pos, Quaternion.identity);
        OnTowerPlaced?.Invoke(CurrentTower.GetComponent<Tower>().GetCost());
        CurrentTower = null;
        PreviewTower = null;
    }

    public void SetTower(GameObject tower){
       photonView.RPC("RPC_SetTower", RpcTarget.All, tower.name);

    }
    [PunRPC]
    public void RPC_SetTower(string tower){
        CurrentTower = PhotonNetwork.Instantiate(tower, Vector3.zero, Quaternion.identity);
        CurrentTower.SetActive(false);
    }



    public void SelectPreview(GameObject preview){
        if(!pv.IsMine){
            return;
        }
        if(preview != null){
            var moneyToSpend = gs.GetCurrency();
            var towerCost = CurrentTower.GetComponent<Tower>().GetCost();
            Debug.Log("Tower Clicked! not null");
            if(moneyToSpend >= towerCost){
                //PreviewTower = tower;
                photonView.RPC("RPC_SelectPreview", RpcTarget.All, preview.name);

                // PreviewTower = tower;
                // PreviewTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
                // //PreviewTower.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                // Debug.Log("Tower Accepted!: " + PreviewTower.name + " " + PreviewTower.name);

            }
        }
    }
    [PunRPC]
    private void RPC_SelectPreview(string preview){
                //PreviewTower = tower;
                PreviewTower = PhotonNetwork.Instantiate(preview, Vector3.zero, Quaternion.identity);
                //PreviewTower.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                //Debug.Log("Tower Accepted!: " + PreviewTower.name + " " + PreviewTower.name);
    }
}
