function [allminSQ,allTQ,allIdx] = IdentifyValidation(x,vx,wind,num,rank)
% �ƶ����ϵ����������֤
% ���룺x������ף�pr����������ʣ�wind����ȣ�num�������û��޸���չʾ��ʶ��������N��vx����֤�����ף�vy����֤������
% �����result��������ʾ��allminSQ����С�ƶ����ϵ��,allTQ��ʶ�������
% result����num+1��*���ʣ���1��Ϊ��ʶ����֤����Ʒ�����ʣ���2����num+1�У�Ϊʶ������Ŀ���Ʒ�����ʣ�������������
% allminSQ��num*��֤����Ʒ��Ŀ
% allTQTQ��num*��֤����Ʒ��Ŀ
	[m,n]=size(vx);
    allminSQ = zeros(num,n);
    allTQ = zeros(num,n);
    allIdx = zeros(num,n);
	[p,w,t] = IdentifyTrain(x,rank);
	for ii=1:n
		y=vx(:,ii);
		[minSQ,TQ,ad] = IdentifyPredictor(x,y,wind,num,p,w,t,1);
		allminSQ(:,ii) = minSQ; 
		allTQ(:,ii) = TQ;
		allIdx(:,ii) = ad;
	end
end