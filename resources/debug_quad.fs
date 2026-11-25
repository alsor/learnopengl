#version 330 core

in vec2 TexCoords;

out vec4 FragColor;

uniform sampler2D depthMap;
// uniform float near_plane;
// uniform float far_plane;

// required for perspective projection
// float LinearizeDepth(float depth)
// {
//     float z = depth * 2.0 - 1.0;
//     return (2.0 * near_plane * far_plane) / (far_plane + near_plane - z * (far_plane - near_plane));
// }

void main()
{
    float depthValue = texture(depthMap, TexCoords).r;
    // FragColor = vec4(vec3(LinearizeDepth(depthValue) / far_plane), 1.0); // for perspective
    FragColor = vec4(vec3(depthValue), 1.0); // for orthographic
}