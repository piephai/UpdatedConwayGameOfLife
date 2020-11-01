---
title: <Life (Part 2)>
author: <Emilie> <Pomeroy> - <n10265813>
date: 26/10/2020
---

## Build Instructions

1. Go to visual studio and build all.
2. Open up the terminal/command prompt.
3. Change directory (cd) into the path that was shown when the program was built on VS in this case you want to get into the netcoreapp3.1 folder.
4. Once you are in the folder you can now use the commands.

## Usage

1. While inside the netcoreapp3.1 folder run the command of: dotnet life.dll to run the program.

2. Dimensions: --dimensions <rows><columns> will change the dimension of the grid. Both rows and columns have to be between 4-48 (inclusive). The default game board will have 16 rows and 16 columns.

3. Periodic: --periodic will change the program to exhibit periodic behaviour. This flag should not be followed by any parameters as it is merely a true or false value. The default settings will not have periodic behaviour.

4. Random Factor: --random <probability> will change the probability of any cell to be initally active. The flag should be followed by one parameter, which should be a float between 0 and 1 (inclusive). The default value is 0.5 (50%).

5. Input File: Supplying an input file through --seed <filename> will set an initial game state. This flag should be followed by a single parameter which must be a valid file path that has the .seed file extension. Users can call --seed and then drag the seed file into the command line arguments or enter their path manually. The default is that no input files are used.

6. Generations: --generations <number> specifies the number of generations in the game. The flag --generations should be followed by a single parameter which is a postivie non-zero integer. The default value is 50 generations.

7. Maximum Update Rate: --max-update <ups> is the maximum number of generational updates per second (ups). This flag must be followed by a single parameter which is a float between 1 and 30 (inclusive). The default value is 5 updates per second.

8. Step Mode: --step causes the program to wait for the user to press the space bar to progress to the next generation. The default is that the program will not run in step mode.

9. Neighbourhood: --neighbour <type> <order> <centre-count> specifies the size and type of neighbourhood to be used. The flag must be followed by three parameters. The first specifying the neighbourhood type which must be one of two strings, wither "moore" or "vonNeumann" (case insensitive). The second specifying order (size) of the neighbourhood, which should be an integer between 1 and 10 (inclusive). The third specifying whether the centre of the neighbourhood is counted as a neighbour or not through the use of a boolean (true or false). The default neighbourhood is a 1st order Moore neighbourhood that doesn’t count the centre.

10. Survival and Birth: --survival <param1> <param2> <param3> ... --birth <param1> <param2> <param3> ... sets the number of live neighbours required for a cell to survive or be born in evolution. Each parameter must be a single integer, or two integers separated by ellipses (. . . ). Integers separated by ellipses represent all numbers within that range (inclusively). The numbers provided must
    be less than or equal to the number of neighbouring cells and non-negative. The deafult is that either 2 or 3 live neighbours are required for a cell to survive and exactly 3 live neighbours are required for a cell to be born.

11. Generational Memory: --memory <number> specifies the number of generations stored in memory for the detection of a steady-state. This flag should be followed by a single parameter which is an integer between 4 and 512 (inclusive). The default is that the program should store 16 generations for detecting a steady-state.

12. Output File: --output <filename> sets the path of the output file. The value must be a valid absolute or relative file path with the .seed file extension. The default is that no output file is set.

13. Ghost Mode: --ghost will set the program to render the game using ghost mode. The flag does not take any parameters and the default is that the program will not run in ghost mode.

## Notes

All of the --command has to start with dotnet life.dll while it is inside the netcoreapp3.1 folder.

The spacebar key is used exclusively to progress through the program and to exit out of the program once the generations are completed.
