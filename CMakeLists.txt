cmake_minimum_required(VERSION 3.10)
project(learnopengl)

set(CMAKE_CXX_STANDARD 17)

find_package(OpenGL REQUIRED)
find_package(glfw3 REQUIRED)

add_executable(app
    main.cpp
    glad.c
)

target_include_directories(app PRIVATE
    ${CMAKE_SOURCE_DIR}/include
)

target_link_libraries(app glfw OpenGL::GL)