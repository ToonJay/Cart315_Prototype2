using UnityEngine;
using UnityEngine.UI;

namespace Invector.vCharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        public virtual void ControlAnimatorRootMotion()
        {
            if (!this.enabled) return;

            if (inputSmooth == Vector3.zero)
            {
                transform.position = animator.rootPosition;
                transform.rotation = animator.rootRotation;
            }

            if (useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
            {
                SetControllerMoveSpeed(freeSpeed);
                SetAnimatorMoveSpeed(freeSpeed);
            }
            else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
            {
                isStrafing = true;
                SetControllerMoveSpeed(strafeSpeed);
                SetAnimatorMoveSpeed(strafeSpeed);
            }

            if (!useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlRotationType()
        {
            if (lockRotation) return;

            bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

            if (validInput)
            {
                // calculate input smooth
                inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

                Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
                RotateToDirection(dir);
            }
        }

        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.right;
                right.y = 0;
                //get the forward direction relative to referenceTransform Right
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
                !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (isGrounded && useContinuousSprint)
                    {
                        isSprinting = !isSprinting;
                    }
                    else if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (!useContinuousSprint && isSprinting)
                {
                    isSprinting = false;
                }
            }
            else if (isSprinting)
            {
                isSprinting = false;
            }
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;

            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            if (collision.collider.gameObject.CompareTag("speed"))
            {
                
                Destroy(collision.collider.gameObject);

                if (freeSpeed.walkSpeed < 8)
                {
                    airSpeed *= 2;

                    freeSpeed.walkSpeed *= 2;
                    freeSpeed.runningSpeed *= 2;
                    freeSpeed.sprintSpeed *= 2;

                    strafeSpeed.walkSpeed *= 2;
                    strafeSpeed.runningSpeed *= 2;
                    strafeSpeed.sprintSpeed *= 2;

                    EnableText2();
                }
            }
            
            if (collision.collider.gameObject.CompareTag("jump"))
            {
                Destroy(collision.collider.gameObject);

                if (airJumpLimit < 2)
                {
                    airJumpLimit += 1;
                    EnableText1();
                }
            }  
            
            if (collision.collider.gameObject.CompareTag("death"))
            {
                EnableText3();
            }

            if (collision.collider.gameObject.CompareTag("win"))
            {
                EnableText4();
            }
        }
        public Text jumpUpgrade;  //Add reference to UI Text here via the inspector
        public Text speedUpgrade;
        public Text death;
        public Text win;
        public Text intro;
        private float timeToAppear = 2f;
        private float timeWhenDisappear;

        //Call to enable the text, which also sets the timer
        public void EnableText1()
        {
            jumpUpgrade.enabled = true;
            speedUpgrade.enabled = false;
            timeWhenDisappear = Time.time + timeToAppear;
        }

        public void EnableText2()
        {
            speedUpgrade.enabled = true;
            jumpUpgrade.enabled = false;
            timeWhenDisappear = Time.time + timeToAppear;
        }

        public void EnableText3()
        {
            death.enabled = true;
            timeWhenDisappear = Time.time + timeToAppear;
        }

        public void EnableText4()
        {
            win.enabled = true;
            timeWhenDisappear = Time.time + timeToAppear;
        }

        //We check every frame if the timer has expired and the text should disappear
        void FixedUpdate()
        {
            if (jumpUpgrade.enabled && (Time.time >= timeWhenDisappear))
            {
                jumpUpgrade.enabled = false;
            }
            if (speedUpgrade.enabled && (Time.time >= timeWhenDisappear))
            {
                speedUpgrade.enabled = false;
            }
            if (death.enabled && (Time.time >= timeWhenDisappear))
            {
                death.enabled = false;
            }
            if (win.enabled && (Time.time >= timeWhenDisappear))
            {
                win.enabled = false;
            }
            
        }

        void Start()
        {
            intro.enabled = true;
        }
    }
}