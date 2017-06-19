using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class MirrorNetworkedVRNode : NetworkBehaviour {
    public VRNode node = VRNode.LeftHand;
    public GameObject handPositionSetterPrefab;

    public void Start()
    {   
    }

    [ContextMenu("Change Scene")]
    private void ChangeScene()
    {
        foreach(var networkBehaviour in FindObjectsOfType<NetworkBehaviour>())
        {
            DontDestroyOnLoad(networkBehaviour.gameObject);
        }
        SceneManager.LoadScene(1);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(TrackHeadCoroutine());
        StartCoroutine(MakeSureSetHand());
    }

    IEnumerator MakeSureSetHand()
    {
        if (NetworkServer.active)
        {
            var left = Instantiate(handPositionSetterPrefab);
            var right = Instantiate(handPositionSetterPrefab);
            NetworkServer.SpawnWithClientAuthority(left, connectionToClient);
            NetworkServer.SpawnWithClientAuthority(right, connectionToClient);

            yield return new WaitForSeconds(1f);
            left.GetComponent<HandPositionSetter>().RpcSetHand(VRNode.LeftHand);
            right.GetComponent<HandPositionSetter>().RpcSetHand(VRNode.RightHand);
        }
    }

    IEnumerator TrackHeadCoroutine()
    {
        while(true)
        {
            transform.rotation = InputTracking.GetLocalRotation(node);
            transform.position = InputTracking.GetLocalPosition(node);
            yield return null;
        }
    }
}
