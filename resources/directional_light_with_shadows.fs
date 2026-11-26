#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;
in vec4 FragPosLightSpace;

out vec4 FragColor;

struct Material {
    sampler2D texture_diffuse1;
};
uniform Material material;

uniform sampler2D shadowMap;

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform DirLight dirLight;

uniform vec3 viewPos;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
float CalcShadow(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir);

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    // directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir);

    FragColor = vec4(result, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);

    // diffuse
    float diff = max(dot(normal, lightDir), 0.0);
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 64);

    vec3 ambient = light.ambient * texture(material.texture_diffuse1, TexCoords).rgb;
    vec3 diffuse = light.diffuse * diff * texture(material.texture_diffuse1, TexCoords).rgb;
    vec3 specular = light.specular * spec * light.specular;

    float shadow = CalcShadow(FragPosLightSpace, normal, lightDir);

    return (ambient + (1.0 - shadow) * (diffuse + specular));
}

float CalcShadow(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{

    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    float currentDepth = projCoords.z;
    float shadow = 0.0;
    if (projCoords.z <= 1.0)
    {
        // float bias = 0.005;
        float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
        shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;
    }
    return shadow;
}
