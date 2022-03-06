all: libpts.so

libpts.so: pts.o
	gcc --shared -o libpts.so pts.o

pts.o: pts.c
	gcc -c -fPIC pts.c -o pts.o