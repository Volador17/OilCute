function net=bann1(x,y,trainf,hm,f1,f2,tn,traino)

%�޼�ؼ���������ѵ������
%net:���ѵ��50�����ڵõ�������������
%x:���׾���
%y:����������
%trainf:ѵ��������ȡֵΪtraingd��traingdm����trainbfg��trainlm�е�һ��
%hm�������ڵ�����
%f1:��һ�㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%f2:�ڶ��㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%tn:ѵ��������
%traino:ѵ��Ŀ��.


aa=inf;
nn=[];
for i=1:50
    net=newff(minmax(x),[hm,1],{f1,f2},trainf); 
    net.trainParam.lr=0.0002;
    net.trainParam.epochs = tn;
    net.trainParam.goal = traino; 
    net=train(net,x,y); 
    a1=sim(net,x);
    seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
    if seca <aa
        aa=seca;
        nn=net;
    end
end 
 
net=nn;