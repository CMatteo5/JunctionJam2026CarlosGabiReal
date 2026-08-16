using UnityEngine;
public class BuildToolPylon : MonoBehaviour
{
    private charecter player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private BoxCollider2D buildCollider;
    private MaterialPropertyBlock propBlock;

    [SerializeField] private float placementRadius = 5f;

    private Color placeableColor;
    private Color blockedColor;

    private int obstructionCount = 0;

    void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<charecter>();
        }
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();

        ColorUtility.TryParseHtmlString("#00B2D1", out placeableColor);
        ColorUtility.TryParseHtmlString("#A41C14", out blockedColor);
    }
    // Update is called once per frame
    void Update()
    {
        if (player.buildMode)
        {
            spriteRenderer.enabled = true;
            animator.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
            animator.enabled = false;
        }
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;

        bool blocked = obstructionCount > 0 || outOfRange() || lineOfSightBlocked() || !player.canAfford();
        player.canPlace = !blocked;
        applyColor(blocked ? blockedColor : placeableColor);
    }
    private bool outOfRange()
    {
        return Vector2.Distance(player.transform.position, transform.position) > placementRadius;
    }
    private bool lineOfSightBlocked()
    {
        Vector2 origin = player.transform.position;
        Vector2 target = transform.position;
        Vector2 direction = target - origin;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction.normalized, direction.magnitude);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject == player.gameObject)
            {
                continue;
            }
            if (hits[i].collider.gameObject == this.gameObject)
            {
                continue;
            }
            if (hits[i].collider.CompareTag("iorn")
                || hits[i].collider.CompareTag("copper")
                || hits[i].collider.CompareTag("door")
                || hits[i].collider.CompareTag("gemstone")
                || hits[i].collider.CompareTag("terain"))
            {
                return true;
            }
        }
        return false;
    }
    private bool isObstructionTag(Collider2D collision)
    {
        return collision.CompareTag("pylon")
            || collision.CompareTag("generator")
            || collision.CompareTag("terain");
    }
    private bool isObstruction(Collider2D collision)
    {
        return collision.gameObject == player.gameObject || isObstructionTag(collision);
    }
    private void applyColor(Color color)
    {
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color);
        spriteRenderer.SetPropertyBlock(propBlock);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isObstruction(collision))
        {
            return;
        }
        obstructionCount++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isObstruction(collision))
        {
            return;
        }
        obstructionCount--;
    }
}