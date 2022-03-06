#define _XOPEN_SOURCE 600
#define _DEFAULT_SOURCE
#include <fcntl.h>
#include <pty.h>
#include <stdlib.h>
#include <termios.h>
#include <unistd.h>

int CreatePTY(int* master, int* slave) {
  int result = openpty(master, slave, NULL, NULL, NULL);
  if (result != 0) return result;
  struct termios termCtl;
  tcgetattr(*master, &termCtl);
  cfmakeraw(&termCtl);
  tcsetattr(*master, TCSANOW, &termCtl);
  return 0;
}

int ReadPTY(int fd, void* output, int length) {
  return read(fd, output, length);
}

int WritePTY(int fd, void* input, int length) {
  return write(fd, input, length);
}

char* GetSlavePTY(int handle) { return ptsname(handle); }

void ClosePTY(int fd) { close(fd); }