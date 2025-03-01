using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private GameManager gm;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(gm.playerAbilityKey)){
            useFormsAbility(playerMovement.getCurForm().formName);
        }
    }

    private void useFormsAbility(string formName){
        switch (formName)
        {
            case "Ball":
                //Will have the dashing ability
                break;
            case "Pogo":
                //Will have the mega jump and arms ability
                break;
        }
    }


    #region Ball Ability
    
    #endregion


}
