#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

out vec4 FragColor;

struct Material {
    sampler2D texture_diffuse1;
};
uniform Material material;

uniform samplerCube shadowMap;

struct PointLight {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
#define NR_POINT_LIGHTS 1
uniform PointLight pointLights[NR_POINT_LIGHTS];

uniform vec3 viewPos;

uniform float far_plane;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
float CalcShadow(PointLight light, vec3 fragPos);


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

    // ambient
    vec3 ambient = light.ambient;
    // diffuse
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff;
    // specular
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 64);
    vec3 specular = light.specular * spec;
    // attenuation
    // float distance = length(light.position - fragPos);
    // float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // float attenuation = 1.0 / (distance * distance);
    // float attenuation = 1.0 / distance;

    // ambient *= attenuation;
    // diffuse *= attenuation;
    // specular *= attenuation;

    // shadow
    float shadow = CalcShadow(light, FragPos);

    return (ambient + (1.0 - shadow) * (diffuse + specular)) * texture(material.texture_diffuse1, TexCoords).rgb;
    // return (ambient + diffuse + specular) * texture(material.texture_diffuse1, TexCoords).rgb;
}

float CalcShadow(PointLight light, vec3 fragPos)
{
    vec3 fragToLight = fragPos - light.position;
    float closestDepth = texture(shadowMap, fragToLight).r;
    closestDepth *= far_plane;
    float currentDepth = length(fragToLight);

    float bias = 0.05;
    // float bias = max(0.008 * (1.0 - dot(normal, lightDir)), 0.0005);

    // PCF
    // float shadow = 0.0;
    // vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    // for (int x = -1; x <= 1; ++x)
    // {
    //     for (int y = -1; y <= 1; ++y)
    //     {
    //         float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
    //         shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
    //     }
    // }
    // shadow /= 9.0;

    // without PCF
    float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;

    return shadow;
}
