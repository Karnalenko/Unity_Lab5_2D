using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField] float movementSpeed = 10f;
    Vector3 movementVector;

    private float gravity = -20f;
    public float jumpHeight = 15f;

    public Transform cameraPosition;
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private AudioSource audioSource;
    public AudioClip woodStepClip;
    public AudioClip concreteStepClip;
    [Range(0.1f, 1f)]
    public float volume = 0.5f;
    private bool woodClipHasPlayed;
    private bool concreteClipHasPlayed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameObject.tag = "Player";
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        if (characterController.isGrounded)
        {
            movementVector = transform.forward * movementSpeed * verticalInput;
            if (Input.GetButtonDown("Jump"))
            {
                movementVector.y = jumpHeight;
            }
        }
        movementVector.y += gravity * Time.deltaTime;
        characterController.Move(movementVector * Time.deltaTime);
        //Debug.Log(Mathf.Abs(movementVector.x));

        Vector2 mouseVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        if (invertX)
        {
            mouseVector.x = -mouseVector.x;
        }
        if (invertY)
        {
            mouseVector.y = -mouseVector.y;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseVector.x, transform.rotation.eulerAngles.z);
        cameraPosition.rotation = Quaternion.Euler(cameraPosition.rotation.eulerAngles + new Vector3(mouseVector.y, 0f, 0f));
    }
    private void FixedUpdate()
    {
        audioSource.volume = volume;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2))
        {
            if (Input.GetButtonDown("Vertical") && (!woodClipHasPlayed || !concreteClipHasPlayed))
            {
                if (hit.collider.tag == ("Wood"))
                {
                    audioSource.clip = woodStepClip;
                    woodClipHasPlayed = true;
                }
                if (hit.collider.tag == ("Ñoncrete"))
                {
                    audioSource.clip = concreteStepClip;
                    concreteClipHasPlayed = true;
                }
                audioSource.Play();
            }
            else if (Input.GetButtonUp("Vertical") || ((hit.collider.tag != "Wood" && woodClipHasPlayed) || (hit.collider.tag != "Ñoncrete" && concreteClipHasPlayed)))
            {
                woodClipHasPlayed = false;
                concreteClipHasPlayed = false;
                audioSource.clip = null;
            }
            if (hit.collider.tag == "LevelPoint")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

}