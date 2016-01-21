#pragma once
#include "stdafx.h"
// Use the namespace you want
namespace js1 {

    class FileLogger {

        public:


            // If you can´t/dont-want-to use C++11, remove the "class" word after enum
            enum e_logType { LOG_ERROR, LOG_WARNING, LOG_INFO };


            // ctor (remove parameters if you don´t need them)
            explicit FileLogger (const char *fname = "projects_logging.txt")
                  :   numWarnings (0U),
                      numErrors (0U)
            {

                myFile.open (fname);

                // Write the first lines
                if (myFile.is_open()) {
                } // if

            }


            // dtor
            ~FileLogger () {

                if (myFile.is_open()) {
                    myFile << std::endl << std::endl;
                    myFile.close();
                } // if

            }

            // Overload << operator using C style strings
            // No need for std::string objects here
            friend FileLogger &operator << (FileLogger &logger, const char *text) {
                logger.myFile << currentDateTime() << ":" << text << std::endl;
                return logger;

            }


            // Make it Non Copyable (or you can inherit from sf::NonCopyable if you want)
            FileLogger (const FileLogger &);
            FileLogger &operator= (const FileLogger &);

            // Get current date/time, format is YYYY-MM-DD.HH:mm:ss
			friend const std::string currentDateTime() {
			    time_t     now = time(0);
			    struct tm  tstruct;
			    char       buf[80];

			    localtime_s(&tstruct, &now);
			    // Visit http://en.cppreference.com/w/cpp/chrono/c/strftime
			    // for more information about date/time format
			    strftime(buf, sizeof(buf), "%Y-%m-%d.%X", &tstruct);

			    return buf;
			}

        private:
            std::ofstream           myFile;

            unsigned int            numWarnings;
            unsigned int            numErrors;

    }; // class end

}  // namespace