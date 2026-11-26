#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

struct Material {
    sampler2D texture_diffuse1;
};
uniform Material material;

struct PointLight {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
#define NR_POINT_LIGHTS 1
uniform PointLight pointLights[NR_POINT_LIGHTS];

uniform vec3 viewPos;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = vec3(0.0);
    // point lights
    for (int i = 0; i < NR_POINT_LIGHTS; i++)
    {
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
    }

    FragColor = vec4(result, 1.0);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);

    // diffuse
    float diff = max(dot(normal, lightDir), 0.0);
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 64);
    // attenuation
    // float distance = length(light.position - fragPos);
    // float attenuation = 1.0 / (distance * distance);
    // float attenuation = 1.0 / distance;

    vec3 ambient = light.ambient * texture(material.texture_diffuse1, TexCoords).rgb;
    vec3 diffuse = light.diffuse * diff * texture(material.texture_diffuse1, TexCoords).rgb;
    vec3 specular = light.specular * spec * light.specular;
    // ambient *= attenuation;
    // diffuse *= attenuation;
    // specular *= attenuation;

    return (ambient + diffuse + specular);
}
