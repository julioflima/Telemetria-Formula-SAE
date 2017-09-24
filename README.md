# Telemetry Prototype Destinated to Formula SAE

***Embedded Automotive System, X-Bee Connected to a C\# Supervisory***

Julio Cesar Ferreira Lima ; 
Melisse P. Cabral

Universidade Federal do Ceará, Sobral, Brasil

julio\_flima@hotmail.com ; 
melissecabral@gmail.com


**Abstract — This report proposes to expose the development of a telemetry
destined to the automotive world, specifically for the Formula SAE
competition. They were used to carry out the project, free software
development as Arduino IDE and Visual Studio. During the report will be
presented diverse environments of developments, aimed not only the
hardware, but as well as the software. Obtaining as well as the results
of the final object, as hardware, software, charts and graphs.**

**Key-words: telemetry; formula sae; visual studio; arduino, xbee.**

## Introduction

In the year 2015, Formula 1 has raised about $ 965 million \[1\], which
justifies all the effort aimed at perfecting and studying techniques and
technologies aimed at the automotive world, to afford this idea, exist
Formula SAE \[2\], which is a competition that happens in some countries
where engineering students compete with cars developed within their
universities. One of the analyzes among the questions is the development
in Electrical Engineering, electrical aspects are so expressive that
justified the creation of a championship focused on electric cars,
E-Racing.

One of the major revolutions that Electrical Engineering caused in the
world of Formula 1 was the acquisition of data for a posteriori
analysis, so the teams were able to improve their cars based on test
data. The second major revolution was the analysis of the values in real
time, which gave greater dynamics and resilience on the trial day. Such
systems are provided by automotive telemetry \[3\] and real-time
performance capture and analysis systems. An example is shown, Figure 1.

![image1](https://user-images.githubusercontent.com/17098382/30785590-438217f4-a13f-11e7-9f7f-3261c665a9ad.png)
<br>
Fig. 1: F1 2002 Telemetry

This work proposes the development of an SAE Formula level of telemetry
prototype (low cost). Using the data acquisition platform, the Atmega
328 microcontroller \[4\], such as the Xbee-Pro SB2 communication
device, and a supervisory system developed in C\#. With the system of
supervision, 14 quantities referring to the car were captured and
analyzed in real time and in later tests, being able to expand, by code,
to more quantities. The radius for receiving data in open field is 1.5
km, and can be expanded using the concept of Mesh or Cluster Tree,
network desing.

In addition to this introductory section, the paper is divided into four
more sections. In Section 2, the methodology used in the work is
described, evidencing the instrumental acquisition process, simulation
process, protocoling process, supervisory system; In Section 3 the
analysis of the results obtained about the prototype and the supervisory
system is presented. Finally, Section 4 sets out the conclusions.

## Methodology

In the telemetry prototype project, the ATmega328 microcontroller was
used only to make the instrumental acquisition. The platform that
supports it is modern with some contemporary programming paradigms,
facilitating code maintenance, the platform is the Arduino IDE \[5\]
encoded in a C language adaptation \[6\] and \[7\]. The X-Bee platform,
either the embedded system, including the I/O's and ADC technology.
However, the programming is not leaded towards large-scale maintenance,
therefore opted to restrict the operating scopes, leaving it only as a
radio for digital data communication.

### Instrumental Acquisition Process

The wide adaptability of the ATmega 328 in the field of instrumentation
makes it a favorite in the worldwide hardware development community.
Whether using digital or analog sensors, being provided by an
open-source community, the identification process becomes less costly.
Even if there is no logical compatibility between the sensors and
microcontroller, since it operates at the TTL (Transistor-Transistor
Logic) level, which comprises voltage values of 0-5V, with only a few
discrete or some more complete components, such as drivers, makes him
adaptable a large scale of sensors. In this work, it was necessary to
some logic level interfaces of the chosen sensors, since the application
scope of the sensors used here are of industrial application that
generally operates in a logic level 0-12V or 0-24V.

1. Digital: The digital acquisitions can be divided into two, the first in discrete states used in drives based on discrete components such as relay, button, inductive, capacitive sensor, such components are discrete since it does not have an operating set-point and only have two states, not actuated and actuated, similar to low level which comprises in the presence of 0V and high level which comprises in the presence of a voltage greater than 3V, in a digital input. Modern sensors also have another ability to communicate with microcontrollers, by sending more data or more complex data through the sending of words through a specific communication. For these sensors a library is needed that identifies the meaning of the received words. These sensors with the modernity and the improvement of techniques have been more and more present in the world of development low cost, but still have a certain complexity of interpretation only some more popular are of public domain. Being part of an open-source community, the use of this type of sensor makes it very easy to develop larger applications, because it would be very costly to create a library that interprets every sensor that one decides to use.

    2. Analogic: Immensely more diffuse, because it refers to the beginnings of industrial instrumentation, this type of sensor is easy to read by means of analog inputs contained in the microcontroller. These inputs interpret the received voltage value, which must not be greater than the internal reference voltage. Although it is simple to acquire this data, the complexity in the interpretation of the transducer, an element that transforms a real-world quantity into an electrical quantity, is sometimes discouraged from being used, requiring some training in analog electronics by the designer, to conditioning the microcontroller interpretation range.

3. Sensors:

    1. Counter: One of the most important parameters in a car is the speed and the RPM, since they provide relations of engine status, steering mode and performance with the relation between speed and rotation, being the first measure with the placement of a sensor in the wheel and the second on the drive shaft. In this way, after detected the rotation, one obtains the angular speed of the motor and wheel, it is not dificult to obtain the engine speed and the speed of the car, (1) and (2). These two parameters can be acquired by an inductive sensor, which when subjected to a voltage at its terminals, varies the state of the output when subjected to variations of presence and no presence of a metal. This type of sensor is of the discrete digital type, being widely used in industrial applications and found largely on a non-TTL level, sometimes requiring a conversion interface, which can be done by a voltage divider \[8\], coupling via voltage regulator or driver, respectively represented by ICs such as LM7805 and ULN2003. Where "v" is the angular speed, "c" is the number of rotations, "T" is the time interval of the acquisitions, "r" is the radius of the wheel and "V" is the speed of the car.
    
    2. States: State variables are decisive for the atuation of some car devices and are also important of be measured, such as the gear position. Although some analogical conditioning that this greatness may have, they basically determine logical states, because they are enough. The gear position is picked up by a button mounted on a device called the paddle shifter, which as soon as it is pressed sends a high level value to the microcontroller that will act on gearbox. However this value will be saved and shown to the pilot, as it is referring to the current gear. The type of configuration required for this type of sensor is a pull-down resistor, aligned to a capacitor in parallel to avoid the effect of noise caused by the mechanical switch \[8\]. This process of removing noise generated by mechanical effects is known as debouncer. An example is shown in Figure 2.

    3. Modern: The DS18B20 sensor is a temperature sensor and has a 1-wire protocol for communication. Among its advantages is the use of only one wire for communication bus, precision and a relative amount of encapsulations that can be found in the market. In this work the immersible stainless steel encapsulation was used, ideal for industrial applications as well as for temperature measurement of oil tank. Another parameter very relevant to the performance of the car is the humidity and temperature of the air, so this data was captured through the DHT22 sensor. It communicates via single-bus protocol, where it also requires only one wire for communication bus. Both protocols are open-source and are available for use across multiple libraries, facilitating implementation..

    4. Traking Position: Some sensors have the function of tracking the position of a device such as the steering wheel, brake, throttle, oil level and gas level. They can be traced by an analog device called a potentiometer \[8\], which undergoes a potential difference at its ends, in case of variation of the axis position, the middle terminal will present a voltage variation, obtaining minimum voltage when connected to GND and maximum when connected to VCC. An example is shown in Figure 3.
    
<img width="200" alt="image2" src="https://user-images.githubusercontent.com/17098382/30785594-598498ec-a13f-11e7-900f-a2eb85b44676.png"><br>
Fig. 2: Schematic Resistor in Pull-Down and Debounce

<img width="200" alt="image3" src="https://user-images.githubusercontent.com/17098382/30785607-b6fdc2b4-a13f-11e7-80af-ff6473ccbb6a.png"><br>
Fig. 3: Schematic of Tracking Position

4. Comunication: The X-bee module needs two interfaces, if not used as the main controller, one to interface its logic level, because although it is TTL, it works at a voltage level of 3.3V, lower than the ATmega 328. And another to communicate with the computer. Both boards are open source and can be developed, but for this work commercial boards were used. In this work, it was tested with a Point-to-Point communication topology, by simply setting internaly the X-Bee to router and the X-Bee ID address that wished to be communicated, in both X-Bee \[9\]. So in a network there would be no collision of information, since the information is addressed. In order to increase the radius of access, just like the one used in Formula 1, you can use topologies such as Zigbee Mesh and Cluster Tree. An example is shown below in figures 5 and 6.

<img width="100" alt="image4" src="https://user-images.githubusercontent.com/17098382/30785610-bfc23da8-a13f-11e7-9c7f-ef0cb6251b4a.png"><br>
Fig. 4: Point-to-Point Network Arrangement

<img width="165" alt="image5-1" src="https://user-images.githubusercontent.com/17098382/30785614-e630ecaa-a13f-11e7-9787-90f2ce895e51.png"><br>
Fig. 5: Zigbee Mesh Network Arrangement

<img width="165" alt="image5-2" src="https://user-images.githubusercontent.com/17098382/30785615-f0a34c96-a13f-11e7-93bb-9a5e5abae754.png"><br>
Fig. 6: Cluster Tree Network Arrangement

Communication with the ATmega 328 is via RS232, so the X-Bee mirrors the
data received at its RS232 input and encodes them into the Zigbee
protocol. On the other hand, the other X-Bee does the reverse process,
sending the bytes in the computer's serial port for the supervisory to
store and process. A better performance was adopted with the highest
possible baudrate, 115200, and the hardware that limited the
transmission capacity was the ATmega 328, as the X-Bee could reach even
higher speeds.

### Protocoling Process

Given the parameters identified in the car, it was realized that only
four digits would be enough to present all the sensors, in some there
was even some exaggeration in the amount of characters. However, by
symmetry of design it was preferred that all had the same format,
whether it is a binary variable or not. Such excesses were not
detrimental in communication, since the speed adopted was very high and
there was a huge advantage in data analysis and programming. Assuming
that zeros on the left do not add anything in mathematical magnitude,
and that each variable has 4 numerical characters, which if multiplied
each by their contribution in units, tens, hundreds, thousands and
finally added, will comprise a number in the decimal base and not more
only one character. In this way a pattern of data arrangement was
developed in a string of characters. An example is shown in Fig. 7.

<img width="450" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785618-fae01e14-a13f-11e7-99e2-3f8b4bc04ae5.png"><br>
Fig. 7: Designed Protocol

Which character "?", was placed to represent the beginning of the
string, the character "!", to represent the end and the character "&",
was used as a spacer between the variables. The values of the variables
will be comprised in the location of the group of four zeros followed,
"0000". Each variable is identified by its position in the string, so
the first variable is the one closest to the initial character and
consequently the last variable is the one closest to the final
character.

### Supervisory System

In software development, a chosen language was C\# \[1\] which although
simple allows rapid development of applications running in the .NET
Framework and maintains the expressiveness and elegance of C.

During the project, there was no adopted architecture model. In the
structure developed in C\#, the forms were divided into: Resources, Form
and Program.

In resources were stored the media used throughout the project, Program
is where all classes and namespaces are initialized, Form is where the
implementation of the entire visual interface and the interactions and
events which are triggered by the forms. Finally, we have the classes
that are where are the classes where all the system functionalities have
been implemented.

The programming method adopted was structured programming, using object
orientation only in the native form that is provided by the development
platform. There is only one class where all features are arranged named
“Cockpick” and it is this same class that makes direct access to the
Database.

<img width="500" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785623-0553e006-a140-11e7-95dd-6fbcff44b8e7.png"><br>
Fig. 8: UML Class Diagram

The data reception by the serial port is done by a native library in C\#
SerialPort \[12\] in the project were used five methods and three
attributes of that library were them:

-   SerialPort.GetPortNames();

-   mySerialPort.Open();

-   mySerialPort.Close();

-   mySerialPort.ReadLine();

-   mySerialPort.WriteLine(string);

-   mySerialPort.BaudRate();

-   mySerialPort.PortName();

-   mySerialPort.IsOpen();

The use of these methods were protected against multiple accesses to
the same port. Placed in a "try / catch" block when trying to access
a port that is already being used the user will receive an
alert message. The applied data processing is the conversion of
bytes received via serial port into measurable numbers and ready to
perform comparisons and calculations.

**“...USERS WITHOUT EXPERIENCE MUST BE ABLE TO USE A SYSTEM IN LESS
THAN TEN MINUTES, OTHERWISE THAT SYSTEM FAILS” \[10\].**|

Interface with captivating colors and self-explanatory buttons and
simple navigation. Where users can use all the features offered with a
lot of objectivity and consequently a minimum amount of clicks, such
features most of the time do not even require input via keyboard. Only
three steps are required to connect the device to the system:

**PORT NAMES → “Select the port of receptor” → OPEN PORT**

After doing so the values are being received and displayed graphically
on a steering wheel on the simulator screen. On the right side, it is
also possible to check the data received by the Arduino at time of
execution.

<img width="500" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785625-1888be62-a140-11e7-9334-496740f51b8e.png"><br>
Fig. 9: Window Cockpick

In this way, it is possible for any member of the team through the
interface have a good understanding of the pilot's vision. Because the
parameters as well as the juxtaposition of the animations of the
steering wheel are a faithful representation of the real steering wheel
designed for the car.

The representation in graphs with the Chart \[13\] component available
in the native C\# library can be used in the use of graphing and also in
the modifications of some parameters can be modified in Visual Studio,
both via interface in the properties as well as in code. To insert the
data in charts was defined the method "LoadGraph()" which is an event
and is called whenever the input values vary. And for each parameter a
point will be inserted in the graph, according to the input.

In the method described above, changes were made to the graph screen
size by means of attributes:

-   CHART.ChartAreas\[0\].AxisX.Maximum
-   CHART.ChartAreas\[0\].AxisX.Minimum

<img width="500" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785629-273e30e0-a140-11e7-82e9-29ac75d8eaeb.png"><br>
Fig. 10: Window Graphs

It is possible for the graph to grow dynamically so that it is never
full on the screen, which makes real-time input variation easier for
visualization.

One of the great benefits of the project would be the subsequent
analysis of the data so that it is possible to identify failure and
where possible the optimization of performance would require that the
data be stored even after the system closes and that it is possible to
read the data to analysis. Knowing this was thinking that the best way
to save this data would be in files that could be read in spreadsheet
editing and viewing software as these are highly diffused and easy to
use by a large part of the public.

Therefore, it was decided to adopt storage in data frames, using XML. It
would be possible for each generated file to be interpreted by softwares
that deal with frameset like LibreOffice.

It was necessary to import some functions and create a dashboard in
order to save data. So, whenever there is a reception of new data it
will be saved to the dashboard by means of the automatic rescue function
or user request.

Due to the need to protect the system against crashes and consequently
the loss of data, an automatic data rescue system was inserted, which,
when starting to read data, also started an internal timer when after
some period of time saves the input data without make it necessary for
user interaction and without interfering with usability.

Also was created an "LerXML" function, which, upon being called, returns
a list of files of the extension .xml present in the current directory
of the system and when we select a file it is loaded into the dashboard
and the graph of the data contained in it is shown.

## Results and Discussions

### Prototype  

Due to the impossibility of designing an entire car for sensor
implementation and testing, due to cost and time, a prototype adapted
from a joystick originally designed for video games was developed. All
the interface hardware has been adapted into the X-Bee's own board
because of its simplicity. Two 5dB antennas were added to improve signal
and transmission quality. Everything was arranged inside a case,
suitable for development. As it was a prototype, there was no concern
with the durability related to mechanical interferences in the future,
caused by the natural vibration of the car. This is a serious concern,
which in case of real development, should be remedied using rubber shock
absorbers for the electrical case support, along with the elimination of
contacts via connectors, requiring that they are all welded onto a
motherboard.

<img width="450" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785635-425cd700-a140-11e7-964d-6884a6815498.jpeg"><br>
Fig. 10: Case of Prototype Opened

<img width="450" alt="image6" src="https://user-images.githubusercontent.com/17098382/30785636-425d4118-a140-11e7-964b-385d8f9aef57.jpeg"><br>
Fig. 12: Final Prototype

### Tests Performed

Due to a tight deadline and software was devised so that the development
occurs quickly so that its functionalities were implemented in time for
delivery. In this way, it was not thought of adopting any architecture
and thus jeopardizing the scalability of the project. However even after
finalizing were inserted some features without compromising the
functioning and fluidity of the system.

After several usage tests, no problems were detected in reading and
manipulating data or files. The system requires the use of an
intermediate video card to support animations that are made in real
time. However, this is not a limiting factor for the use, causing in
some cases only a certain delay in the animations.

The system went through several code usage tests and code analysis after
finalization and no critical issues were found.

## Conclusions

The prototype met all the proposed objectives. Although the project is only a prototype, with the advent of this, given by the arduous process until then, the implementation of future sensors will have very reduced difficulty, being limited basically to only an implementation of sensors in the microcontroller.

With a low cost of production and excellent accuracy in sending data, telemetry has achieved great results in terms of cost-benefit. Since this type of technology is used by the most expensive sport in the world, then it has high valuation within the market, a factor that makes cost-benefit a differential. Another differential in this product was its ability to carry out all the acquisitions in an easy way in relation to the user, guaranteeing a good user experience. Another aspect that shows itself as a differential of the product is its non-dependence on other software for plotting graphs later acquired by the platform, and in contrast the possibility of exporting to other software like Excel, since tables in "XML" mode also can be incorporated by it. Even because it is a simple hardware, low cost, it was possible to notice an immense robustness, since past months of the project realization, still works and even was already the object of implementation of another project, based on its source code. 

The prototype should and will be improved as regards its latency in response to the computer, but such latency given the time of slow analysis, intrinsic characteristic of the human, does not compromise at all its analysis. It would also be interesting to program the Atmega 328 code at Arduino IDE using the object-oriented programming paradigm (OOP), in order to make code simpler to understand and in a more modern way, since OOP is the most appropriate currently being programmed, avoiding many repetitions of lines of code.

As in all aspects of the project, other open-source projects were considered, this project can also be seen as open-source and was made available on the GitHub platform \[14\]. 

## References

1.  AutoSport Magazine, “Formula 1 team payments for 2016 revealed,” 2016. \[Online\]. Available: http://www.autosport.com/news/report.php/id/123649

2.  SAE Brasil, “O que é o fórmula?”. \[Online\]. Available: http://www.saebrasil.org.br/eventos/programas\_estudantis/formula.aspx

3.  Formula 1 Dictionary, “Telemetry”. \[Online\]. Available: http://www.formula1-dictionary.net/telemetry.html

4.  Arduino, “Language Reference”. \[Online\]. Available: https://www.arduino.cc/en/Reference/HomePage

5.  J. W. Nilsson and S. A. Riedel. Circuitos eletricos. 8ª edição São Paulo: Pearson Prentice Hall, 575p, 2009.

6.  Digi X-CTU, 2017. \[Online\]. Available: www.digi.com

7. Nelson, Design de Interação, 1980.

8. P. Deitel and H. Deitel, Visual C\# How to Program, 6th Edition.

9. Microsoft, “C\# Guide”, Microsoft, 2016. \[Online\].

10. Microsoft, “Visual Studio Documentation”, 2017. \[Online\]. Available: https://docs.microsoft.com/en-us/visualstudio
