import React, { Component } from 'react';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Image, ScrollView, TouchableWithoutFeedback, Dimensions } from 'react-native';
import { Container, Header, Left, Right, Body, Title, Content, Footer, FooterTab, Icon, Button} from 'native-base';
import { LinearGradient } from 'expo-linear-gradient';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/MaterialCommunityIcons';
import AsyncStorage from '@react-native-community/async-storage';
import styles from '../styles/HomeScreenStyleSheet';
import stylesLandscape from '../styles/HomeScreenLandscape';

export default class AssessmentScore extends Component {
  constructor(props) {
    super(props);
    this.state= {
      modal: 'false',
      modalColor: "#66bb77",
      notificationColor: "#66bb77",
      menu: 'false',
      username: '',
      correct: 0, incorrect: 3, percentage: 0,
      scoreColor: '#00CD98'
    }
  }
  //Set username from local storage
  setUsername = async () => {
    const userData = await AsyncStorage.getItem('userData');
    const parseData = JSON.parse(userData);
    const user = parseData.username;
    this.setState({username: user});
  }
  //Clear local storage when signing out
  clearDataToStorage = (token, userId, username) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId, username: username}));
  };
  //Clear current score when navigating from screen
  clearScore = () => {
    AsyncStorage.setItem('score', JSON.stringify({correct: 0, incorrect: 3, percentage: 0}));
  }

  //Get correct, incorrect, and percentage from local storage
  handleScore = async () => {
      const data =  await AsyncStorage.getItem('score');
      console.log(data);
      const parseData = JSON.parse(data);
      const correct = parseData.correct;
      const incorrect = parseData.incorrect;
      const percentage = parseData.percentage;
      console.log(parseData);
      console.log(percentage);
      this.setState({correct: correct});
      this.setState({incorrect: incorrect});
      this.setState({percentage: percentage});
      //Set color for each score range
      if(percentage < 35)
      {
        this.setState({scoreColor: '#F64000'});
      }
      else if(percentage < 68)
      {
        this.setState({scoreColor: '#F6B900'});
      }
      else
      {
        this.setState({scoreColor: '#00CD98'});
      }

  }
  componentDidMount() {
    this.handleScore();
  }

  //UI for Assessment Score screen
  render() {
    return (
      <Container>
        {/*Notification modal*/}
        <Modal
          animationType="fade"
          transparent={true}
          visible={this.state.modal}
          supportedOrientations={['portrait', 'landscape']}
        >
          <TouchableOpacity
            style={styles.modalContainer}
            activeOpacity={1}
            onPressOut={() => {this.setState({modal: false, notificationColor: "#555555"})}}
          >
              <TouchableWithoutFeedback>
                <View style={styles.modal}>
                  <View style={styles.modalArrow}>
                    <Icon2 name="triangle" size= {30} color= {"white"} style= {{paddingTop: 15}}/>
                  </View>
                  <View style={styles.modalContent}>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Assessment Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="clipboard" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                    {/*Notification item*/}
                    <View style={styles.modalNotification}>
                      <View style={styles.modalNotificationHeader}>
                        <Text style= {[styles.textNotification, {color: this.state.notificationColor}]} numberOfLines= {2}> Welcome Notification... </Text>
                      </View>
                      <View style={styles.modalNotificationFooter}>
                        <Icon1 name="user" size= {15} color= {"#555555"}/>
                        <Text style= {styles.textNotificationFooter}> Time stamp... </Text>
                      </View>
                    </View>
                  </View>
                </View>
              </TouchableWithoutFeedback>
          </TouchableOpacity>
        </Modal>
        {/*Side Menu modal*/}
        <Modal
          animationType="slide"
          transparent={true}
          visible={this.state.menu}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style={styles.menuContainer}>
            <View style={styles.menu}>
              {/*Close menu button*/}
              <View style={styles.menuClose}>
                <Icon2 name="close" size= {40} color= {"black"} onPress= {() => this.setState({menu: false})} style={{paddingRight: 10, paddingBottom: 5}}/>
              </View>
              {/*Menu profile*/}
              <View style={styles.menuProfile}>
                <View style={styles.menuProfileImage}>
                  <Icon1 name="user" size= {100} color= {"black"}/>
                </View>
                <View style={styles.menuProfileName}>
                  <Text style={styles.textMenuProfile}> {this.state.username} </Text>
                  {/*Sign out button*/}
                  <TouchableOpacity
                    onPress={() => {
                      this.clearDataToStorage(null, null, null);
                      this.clearScore();
                      this.props.navigation.navigate({routeName: 'Login'});
                      this.setState({menu: false});
                    }}
                  >
                    <Text style={styles.textMenuSignOut}> Sign Out </Text>
                  </TouchableOpacity>
                </View>
              </View>
              <View style={styles.menuContent}>
                {/*Analysis*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Analysis </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Settings*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Settings </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*Help*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> Help </Text>
                    </View>
                  </TouchableOpacity>
                </View>
                {/*What We Do*/}
                <View style={styles.menuItem}>
                  <TouchableOpacity>
                    <View style={styles.menuItemTitle}>
                      <Icon1 name="user" size= {20} color= {"#77bb77"} style={{paddingLeft: 10}}/>
                      <Text style= {styles.textMenuOptions}> What We Do </Text>
                    </View>
                  </TouchableOpacity>
                </View>
              </View>
            </View>
          </View>
        </Modal>
        {/*Header*/}
        <Header style={{backgroundColor: 'white', borderBottomWidth: 0}}>
          <Left>
              <Button transparent>
                <Icon1 name='align-left' size= {25} color= {"black"} onPress={() => {
                  this.setState({menu: true});
                  this.setUsername();
                }}/>
              </Button>
            </Left>
            <Body>
              <Image source= {require('C:/Users/dadir/AppSignUp/images/cognifyx_infinitum_logo.png')}/>
            </Body>
            <Right>
              <Button transparent>
                <Icon1 name='bell' size= {25} color= {this.state.modalColor} solid= {true} onPress={() => {
                  this.setState({modal: true, modalColor: "black"})
                }}/>
              </Button>
            </Right>
        </Header>
        {/*Assessment Score*/}
        <View style={stylesAssessment.container}>
          {/*Assessment header*/}
          <View style={stylesAssessment.header}>
            <View style={stylesAssessment.headerTitle}>
              <Text style={stylesAssessment.textAssessment}> ASSESSMENT </Text>
            </View>
          </View>
          <View style={stylesAssessment.content}>
            <View style={stylesAssessment.image}>
              <Image source= {require('C:/Users/dadir/AppSignUp/images/brain.png')}/>
            </View>
            {/*Assessment Results*/}
            <View style={stylesAssessment.results}>
              <View style={stylesAssessment.resultsItem}>
                <View style={stylesAssessment.resultsIcon}>
                  <Icon2 name="help-circle-outline" size= {40} color= {"#A2C0D4"}/>
                </View>
                <Text style={stylesAssessment.textResults}> 3/3</Text>
                <Text style={stylesAssessment.textResultsDescription}> questions answered </Text>
              </View>
              <View style={stylesAssessment.resultsItem}>
                <View style={stylesAssessment.resultsIcon}>
                  <Icon2 name="check-circle-outline" size= {40} color= {"#4CD964"}/>
                </View>
                <Text style={stylesAssessment.textResults}> {this.state.correct}/3</Text>
                <Text style={stylesAssessment.textResultsDescription}> correct </Text>
              </View>
              <View style={stylesAssessment.resultsItem}>
                <View style={stylesAssessment.resultsIcon}>
                  <Icon2 name="close-circle-outline" size= {40} color= {"red"}/>
                </View>
                <Text style={stylesAssessment.textResults}> {this.state.incorrect}/3</Text>
                <Text style={stylesAssessment.textResultsDescription}> incorrect </Text>
              </View>
            </View>
            <View style={stylesAssessment.percentage}>
              <View style={[stylesAssessment.percentageWheel, {backgroundColor: this.state.scoreColor}]}>
                <View style={stylesAssessment.percentageContent}>
                  <Text style={[stylesAssessment.textPercentage, {color: this.state.scoreColor}]}> {this.state.percentage}% </Text>
                </View>
              </View>
            </View>
          </View>
        </View>
        {/*Bottom tabs*/}
        <Footer>
          <FooterTab>
            <Button
              onPress={() => {
                this.props.navigation.navigate({routeName: 'HomeScreen'});
                this.clearScore();
              }}
            >
              <Icon1 name="home" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Home</Text>
              </View>
            </Button>
            <Button
              onPress={() => {
                this.props.navigation.navigate({routeName: 'Assessment'});
                this.clearScore();
              }}
            >
              <Icon1 name="clipboard" size= {25} color= {"#66bb77"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooterHighlight}>Assessment</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => {
              this.props.navigation.navigate({routeName: 'BrainGames'});
              this.clearScore();
            }}
            >
              <Icon1 name="brain" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
                <Text style= {styles.textFooter}>Brain</Text>
                <Text style= {styles.textFooter}>Games</Text>
              </View>
            </Button>
            <Button
            style= {{paddingTop: 20}}
            onPress={() => {
              this.props.navigation.navigate({routeName: 'ActiveListening'});
              this.clearScore();
            }}
            >
              <Icon1 name="assistive-listening-systems" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Active</Text>
              <Text style= {styles.textFooter}>Listening</Text>
              </View>
            </Button>
            <Button
              onPress={() => {
                this.props.navigation.navigate({routeName: 'Newsroom'});
                this.clearScore();
              }}
            >
              <Icon1 name="newspaper" size= {25} color= {"#888888"}/>
              <View style= {styles.footer}>
              <Text style= {styles.textFooter}>Newsroom</Text>
              </View>
            </Button>
          </FooterTab>
        </Footer>
      </Container>
    );
  }
}

const stylesAssessment = StyleSheet.create({
  container: {
    flex: 1
  },
  header: {
    //backgroundColor: 'purple',
    height: 80,
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    justifyContent: 'center',
    alignItems: 'center'
  },
  headerTitle: {
    height: 50,
    width: 160,
    backgroundColor: "#66bb77",
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center'
  },
  content: {
    //backgroundColor: 'black',
    flex: 1,
    alignItems: 'center'
  },
  image: {
    //backgroundColor: 'blue',
    height: 250,
    width: 400,
    justifyContent: 'center',
    alignItems: 'center'
  },
  results: {
    //backgroundColor: 'yellow',
    height: 200,
    width: 400,
    justifyContent: 'center',
    alignItems: 'center',

  },
  resultsItem: {
    //backgroundColor: 'white',
    height: 50,
    width: 320,
    borderBottomWidth: 2,
    borderColor: "#888888",
    marginTop: 10,
    flexDirection: 'row',
    alignItems: 'flex-end'
  },
  resultsIcon: {
    //backgroundColor: 'purple',
    height: 50,
    width: 50,
    justifyContent: 'flex-end',
    alignItems: 'center'
  },
  percentage: {
    //backgroundColor: 'green',
    height: 170,
    width: 400,
    justifyContent: 'center',
    alignItems: 'center'
  },
  percentageWheel: {
    backgroundColor: '#00CD98',
    height: 100,
    width: 100,
    borderRadius: 50,
    justifyContent: 'center',
    alignItems: 'center'
  },
  percentageContent: {
    backgroundColor: 'white',
    height: 80,
    width: 80,
    borderRadius: 40,
    justifyContent: 'center',
    alignItems: 'center'
  },
  textAssessment: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  textResults: {
    fontSize: 18,
    fontWeight: 'bold',
    color: "#D6716B",
    marginHorizontal: 5,
    marginBottom: 5
  },
  textResultsDescription: {
    fontSize: 18,
    color: "#888888",
    marginBottom: 5
  },
  textPercentage: {
    fontSize: 18,
    fontWeight: 'bold',
    color: "#00CD98",
  },
})
