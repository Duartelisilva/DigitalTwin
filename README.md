For my thesis project, I developed a digital twin of a 1:43 scale autonomous racing system using Unity. I created the virtual environment and assets, and integrated them into a functional simulation.

I modeled the race track and race cars in **Blender** to accurately represent the real-world system and imported them into **Unity** to build the digital twin environment. In Unity, I configured the scene, scaling, physics, and interactions required for realistic behavior. I developed scripts, in **C#**, to send control commands to the real cars via TCP, simulate the virtual cars using the same dynamics as the real vehicles, and perform real-time monitoring of the physical system. This digital twin was also prepared for the development of machine learning algorithms within its simulation. The resulting platform enables real-time visualization, monitoring, and simulation of the race cars, providing a safe and flexible foundation for testing control strategies and training machine-learning models.

<img width="1087" height="650" alt="image" src="https://github.com/user-attachments/assets/d7ab6a3a-531b-43ce-be64-d2297ef731ba" />
<br>

Checkpoints were added to the race track in order to ensure the cars wouldn't cheat if they crossed over terrain:
<img width="994" height="651" alt="image" src="https://github.com/user-attachments/assets/d3fce183-e4e8-4ab0-bfda-82e12353ee1a" />
<br>

Statistics were calculated and displayed on the screen, with the intent to see details on the car's behavior on the race track in real time.
<img width="1415" height="796" alt="image" src="https://github.com/user-attachments/assets/29b4bf9d-f87e-4b60-8048-7036571655ec" />
<br>

Various values were also stored in a file, like the speed and position of each car, for a given timestamp, so that they could be used for Machine Learning development using these values.
<img width="1212" height="704" alt="image" src="https://github.com/user-attachments/assets/c30d5c15-2882-4c5f-9f25-6aacf07194fa" />
<br>

Unity could communicate with the real-life cars, and thus when an instruction was sent, the virtual counterpart would move accordingly, mirroring the movement and position of each other.
<img width="1563" height="881" alt="image" src="https://github.com/user-attachments/assets/fb712f07-2200-4a31-b790-70d0174b651d" />
