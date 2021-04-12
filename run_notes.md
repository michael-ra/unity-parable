# Run Notes

1. ID - first_self_play  
    * Error: Chaser and runner received positive rewards (ratio current/max_env) for each timestep.
2. ID - second_self_play
    * Fixed above error.
3. ID - third_self_play
    * Fix rotating raycast sensor and switch to capsule collider.
4. ID - fourth_self_play
    * Increase wall and border height. Increase time in environment without obstacles. Adjust capsule collider. Create goal for runner. Disable per timestep rewards.
5. ID - fifth_self_play
    * Add existential reward per timestep. Adjust win reward for duration until win. Increase max steps, buffer size, batch size and network size.